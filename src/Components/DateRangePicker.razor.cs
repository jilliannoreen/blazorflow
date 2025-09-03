using BlazorFlow.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorFlow.Components;

public partial class DateRangePicker : ComponentBase
{
    [Inject] public IJSRuntime JsRuntime { get; set; }
    [Parameter] public DateRange Value { get; set; } = new();
    [Parameter] public EventCallback<DateRange> ValueChanged { get; set; }
    [Parameter] public DateTime? MinDate { get; set; }
    [Parameter] public DateTime? MaxDate { get; set; }
    
    private string TriggerId = $"trigger-{Guid.NewGuid()}";
    private string PopoverId = $"popover-{Guid.NewGuid()}";
    private bool ShowCalendar { get; set; }
    private bool OpenUpwards { get; set; }
    private bool AlignRight { get; set; }
    private DateTime LeftMonth { get; set; } = DateTime.Today;
    private DateTime RightMonth => LeftMonth.AddMonths(1);
    private string[] DaysOfWeek { get; } = { "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat" };

    private string DisplayRange =>
        Value.Start.HasValue && Value.End.HasValue
            ? $"{Value.Start:MM/dd/yyyy} - {Value.End:MM/dd/yyyy}"
            : "";

    private async Task ToggleCalendar()
    {
        ShowCalendar = !ShowCalendar;
        if (ShowCalendar)
        {
            var viewportWidth = await JsRuntime.InvokeAsync<double>("eval", "window.innerWidth");
            var viewportHeight = await JsRuntime.InvokeAsync<double>("eval", "window.innerHeight");

            var rect = await JsRuntime.InvokeAsync<BoundingRect>("eval", $"document.querySelector('#{TriggerId}').getBoundingClientRect()");
            OpenUpwards = (viewportHeight - rect.Bottom) < 300;
            AlignRight = (viewportWidth - rect.Right) < 350;
        }
    }
    
    private string CalendarPositionClass => ClassBuilder
        .Default("absolute bg-white rounded shadow p-4 z-50")
        .AddClass(OpenUpwards ? "bottom-full mb-2" : "top-full mt-2") 
        .AddClass(AlignRight ? "right-0" : "left-0")
        .Build();
    
    private void PreviousMonth() => LeftMonth = LeftMonth.AddMonths(-1);
    private void NextMonth() => LeftMonth = LeftMonth.AddMonths(1);

    private IEnumerable<IEnumerable<DateTime?>> GetWeeks(DateTime month)
    {
        var firstDay = new DateTime(month.Year, month.Month, 1);
        var start = firstDay.AddDays(-(int)firstDay.DayOfWeek);

        for (int week = 0; week < 6; week++)
        {
            yield return Enumerable.Range(0, 7).Select(i =>
            {
                var day = start.AddDays(week * 7 + i);
                return day.Month == month.Month ? day : (DateTime?)null;
            });
        }
    }

    private async Task SelectDate(DateTime? day)
    {
        if (!day.HasValue || !IsWithinRange(day.Value)) return; 

        if (Value.Start == null || (Value.Start != null && Value.End != null))
        {
            Value.Start = day;
            Value.End = null;
        }
        else if (Value.End == null)
        {
            if (day < Value.Start)
                (Value.Start, Value.End) = (day, Value.Start);
            else
                Value.End = day.Value.AddDays(1).AddTicks(-1);;

            // Auto-close once range is complete
            if (Value.Start.HasValue && Value.End.HasValue)
            {
                ShowCalendar = false;
                await ValueChanged.InvokeAsync(Value);
            } 
        }
    }
    
    private bool IsWithinRange(DateTime day)
    {
        if (MinDate.HasValue && day < MinDate.Value) return false;
        if (MaxDate.HasValue && day > MaxDate.Value) return false;
        return true;
    }

    private string GetDayClasses(DateTime? day)
    {
        var builder = ClassBuilder
            .Default("text-center text-sm font-normal px-3 py-2");

        if (day.HasValue)
        {
            if (!IsWithinRange(day.Value))
            {
                builder.AddClass("text-gray-400 cursor-not-allowed"); 
            }
            else
            {
                builder.AddClass("cursor-pointer")
                    .AddClass("bg-(--primary) text-(--primary-foreground)", (Value.Start == day || Value.End == day))
                    .AddClass("rounded-l-lg", Value.Start == day)
                    .AddClass("rounded-r-lg", Value.End == day)
                    .AddClass("bg-(--primary)/25", Value.Start != null && Value.End != null && day > Value.Start && day < Value.End)
                    .AddClass("hover:bg-(--primary)/25", Value.Start != null && day > Value.Start);
            }
        }
        return builder.Build();
    }
}

public class DateRange
{
    public DateTime? Start { get; set; }
    public DateTime? End { get; set; }

}

public class BoundingRect
{
    public double Top { get; set; }
    public double Bottom { get; set; }
    public double Left { get; set; }
    public double Right { get; set; }
    public double Width { get; set; }
    public double Height { get; set; }
}
