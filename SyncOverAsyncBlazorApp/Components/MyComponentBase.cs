using Microsoft.AspNetCore.Components;

namespace SyncOverAsyncBlazorApp.Components;

public class MyComponentBase : ComponentBase, IHandleEvent
{
    async Task IHandleEvent.HandleEventAsync(EventCallbackWorkItem item, object? arg)
    {
        // イベントハンドラが同期処理である想定なので Task.Run .でラップする
        var task = Task.Run(() => item.InvokeAsync(arg));
        await InvokeAsync(() => StateHasChanged());

        if (task.IsCompleted) return;

        await HandleUncompletedEventAsync(task);
    }

    private async Task HandleUncompletedEventAsync(Task task)
    {
        try
        {
            await task;
        }
        catch
        {
            if (task.IsCanceled) return;
            
            throw;
        }

        await InvokeAsync(() => StateHasChanged());
    }
}
