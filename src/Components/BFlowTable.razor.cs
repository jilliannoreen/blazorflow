using BlazorFlow.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorFlow.Components;

public partial class BFlowTable<TItem, TRequest> : ComponentBase
{
    [Inject] private IJSRuntime JSRuntime { get; set; } = default!;

    [Parameter] public required RenderFragment HeaderTemplate { get; set; }
    [Parameter] public required RenderFragment<TItem> RowTemplate { get; set; }
    [Parameter] public EventCallback<TItem> OnRowClick { get; set; }
    [Parameter] public string RowClass { get; set; }
    [Parameter] public string HeaderClass { get; set; }

    [Parameter] public required Func<TRequest, Task<TableResponse<TItem>>> LoadData { get; set; }
    [Parameter] public required Func<TableRequestParams, TRequest> BuildRequest { get; set; }

    [Parameter] public bool StickyHeader { get; set; } = false;
    [Parameter] public bool UseCursorPagination { get; set; } = true;
    [Parameter] public Func<object, string?>? GetCursorFromItem { get; set; }
    [Parameter] public IEnumerable<TItem>? Items { get; set; }


    private ElementReference _tableContainer;

    private List<TItem> _items = [];
    private bool _isLoading = false;
    private bool _firstLoad = true;
    private bool _hasMoreData = false;
    private bool _isEmpty = false;

    // Cursor pagination state
    private string? _cursor = null;

    // Page-based pagination state
    private int _currentPage = 1;
    private int _pageSize = 10;
    private int _totalPages = 1;

    // Row height estimate for JS interop
    private const int RowHeight = 58;

    private string TableRowClass => ClassBuilder
        .Default("odd:bg-gray-50 even:bg-white hover:bg-blue-50")
        .AddClass("cursor-pointer", OnRowClick.HasDelegate)
        .AddClass(RowClass)
        .Build();
    private string TableHeaderClass => ClassBuilder
        .Default("text-xs text-gray-700 font-normal bg-slate-200")
        .AddClass("sticky top-0 z-10 ", StickyHeader)
        .AddClass(HeaderClass)
        .Build();

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && _firstLoad)
        {
            // Estimate how many rows to display based on container height
            var estimatedRowCount = await JSRuntime.InvokeAsync<int>(
                "flowbiteBlazorInterop.table.getAvailableRowCount",
                CancellationToken.None,
                [_tableContainer, RowHeight ]
            );

            _pageSize = estimatedRowCount;

            await LoadInitialAsync();
            StateHasChanged();
        }
    }

    /// <summary>
    /// Handles first data load.
    /// </summary>
    private async Task LoadInitialAsync()
    {
        
        _isLoading = true;
        _items.Clear();
        _cursor = null;
        _currentPage = 1;

        if (UseCursorPagination)
            await LoadCursorPageAsync();
        else 
            await LoadPagedPageAsync();
            
        _isLoading = false;
        _firstLoad = false;
    }

    /// <summary>
    /// Fetches next page in cursor pagination.
    /// </summary>
    private async Task ShowMoreAsync()
    {
        if (_isLoading || !_hasMoreData) return;

        _isLoading = true;
        await LoadCursorPageAsync();
        _isLoading = false;
    }

    /// <summary>
    /// Loads a new cursor-based page.
    /// </summary>
    private async Task LoadCursorPageAsync()
    {
        var request = BuildRequest.Invoke(new TableRequestParams()
        {
            Cursor = _cursor ?? string.Empty,
            Size = _pageSize + 1,
        });

        var response = await LoadData(request);
        if (response?.Data is null)
        {
            _isEmpty = !_items.Any();
            return;
        }
        
        var resultList = response.Data.ToList();
        if (resultList.Count > 0)
        {
            var items = resultList.Take(_pageSize).ToList();
            _items.AddRange(items);
            
            var lastItem = items.Last();
            if (lastItem != null) 
                _cursor = GetCursorFromItem?.Invoke(lastItem);
        }

        _hasMoreData = resultList.Count > _pageSize;
        _isEmpty = !_items.Any();
    }

    /// <summary>
    /// Go to a specific page in page-based pagination.
    /// </summary>
    private async Task GoToPage(int page)
    {
        if (_isLoading || page < 1 || (page > _totalPages && _totalPages != 0)) return;

        _currentPage = page;
        _isLoading = true;

        await LoadPagedPageAsync();

        _isLoading = false;
    }

    /// <summary>
    /// Loads a new page in page-based pagination.
    /// </summary>
    private async Task LoadPagedPageAsync()
    {
        var request = BuildRequest.Invoke(new TableRequestParams()
        {
            Size = _pageSize,
            PageIndex = _currentPage
        });

        var response = await LoadData(request);
        var resultList = response.Data.ToList();

        _items = resultList;
        _isEmpty = !_items.Any();


        _totalPages = response.Count;
    }
    
    
    public async Task ReloadAsync()
    {
        _firstLoad = true;
        _hasMoreData = false;
        await LoadInitialAsync(); // Example: re-fetch data
    }
}

public class TableRequestParams
{
    public string Search { get; set; }
    public int Size { get; set; }
    public string Cursor { get; set; } = string.Empty;
    public int PageIndex { get; set; } // for page-based
}

public class TableResponse<T>
{
    public IEnumerable<T> Data { get; set; } 
    public int Count { get; set; } 
}