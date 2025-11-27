using BlazorFlow.Enums;
using BlazorFlow.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorFlow.Components;

public partial class Table<TItem, TRequest> : ComponentBase
{
    [Inject] private IJSRuntime JSRuntime { get; set; } = default!;

    [Parameter] public required RenderFragment HeaderTemplate { get; set; }
    [Parameter] public required RenderFragment<TItem> RowTemplate { get; set; }
    [Parameter] public EventCallback<TItem> OnRowClick { get; set; }
    [Parameter] public RenderFragment EmptyState { get; set; }
    [Parameter] public string RowClass { get; set; }
    [Parameter] public string HeaderClass { get; set; } 
    [Parameter] public string Class { get; set; }
    [Parameter] public int HeightOffset { get; set; }

    [Parameter] public required Func<TRequest, Task<TableResponse<TItem>>> LoadData { get; set; }
    [Parameter] public required Func<TableRequestParams, TRequest> BuildRequest { get; set; }

    [Parameter] public bool StickyHeader { get; set; } = false;
    [Parameter] public bool IsShowMoreEnabled { get; set; } = true;
    [Parameter] public Func<object, string?>? GetCursorFromItem { get; set; }
    [Parameter] public PaginationMode PaginationMode { get; set; } = PaginationMode.Offset;


    private ElementReference _tableContainer, _tableHeader;

    private HashSet<TItem> _items = [];
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
    private const int RowHeight = 54;

    private string TableRowClass => ClassBuilder
        .Default("focus:outline-none odd:bg-gray-50 even:bg-white hover:bg-blue-50")
        .AddClass("cursor-pointer", OnRowClick.HasDelegate)
        .AddClass(string.IsNullOrWhiteSpace(RowClass) ? "even:bg-(--gray-50)! odd:bg-white!" : RowClass)
        .Build();
    private string TableHeaderClass => ClassBuilder
        .Default("text-xs font-normal")
        .AddClass("sticky top-0 z-10 ", StickyHeader)
        .AddClass(string.IsNullOrWhiteSpace(HeaderClass) ? "bg-(--gray-400) text-white" : HeaderClass)
        .Build();

    private string TableClass => ClassBuilder
        .Default("w-full text-sm text-left text-(--gray-900)")
        .AddClass("!h-[calc(100%-70px]", _isEmpty)
        .AddClass(Class)
        .Build();

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && _firstLoad)
        {
            // Estimate how many rows to display based on container height
            var estimatedRowCount = await JSRuntime.InvokeAsync<int>(
                "blazorFlowInterop.table.getAvailableRowCount",
                CancellationToken.None,
                [_tableContainer, _tableHeader, HeightOffset, RowHeight ]
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

        if (IsShowMoreEnabled)
        {
            switch (PaginationMode)
            {
                case PaginationMode.Cursor:
                    await LoadByCursorAsync();
                    break;
                default:
                    await LoadByOffsetAsync();
                    break;
            }
        }
        else 
            await LoadPagedPageAsync();
            
        _isLoading = false;
        _firstLoad = false;
        StateHasChanged();
    }

    /// <summary>
    /// Fetches next page in cursor pagination.
    /// </summary>
    private async Task ShowMoreAsync()
    {
        if (_isLoading || !_hasMoreData) return;

        _isLoading = true;
        if (PaginationMode == PaginationMode.Cursor)
            await LoadByCursorAsync();
        else
        {
            _currentPage++;
            await LoadByOffsetAsync();
        }

        _isLoading = false;
    }

    /// <summary>
    /// Loads a new cursor-based page.
    /// </summary>
    private async Task LoadByCursorAsync()
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
            _items.UnionWith(items);
            
            var lastItem = items.Last();
            if (lastItem != null) 
                _cursor = GetCursorFromItem?.Invoke(lastItem);
        }

        _hasMoreData = resultList.Count > _pageSize;
        _isEmpty = !_items.Any();
    }
    
    private async Task LoadByOffsetAsync()
    {
        var request = BuildRequest.Invoke(new TableRequestParams()
        {
            PageIndex = _currentPage,
            Size = _pageSize,
        });

        var response = await LoadData(request);
        if (response?.Data is null)
        {
            _isEmpty = !_items.Any();
            return;
        }
        
        var resultList = response.Data.ToList();
        if (resultList.Count > 0)
            _items.UnionWith(resultList);
        
        _hasMoreData = response.Count > _items.Count();
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

        _items = [..resultList];
        _isEmpty = !_items.Any();
        _totalPages = response.Count; 
    }
    
    
    public async Task ReloadAsync()
    {
        _firstLoad = true;
        _hasMoreData = false;
        await LoadInitialAsync(); 
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