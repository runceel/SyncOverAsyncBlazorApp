using Microsoft.AspNetCore.Components;

namespace SyncOverAsyncBlazorApp.Components;

public class MyComponentBase : ComponentBase, IHandleEvent
{
    Task IHandleEvent.HandleEventAsync(EventCallbackWorkItem item, object? arg)
    {
        // イベントハンドラが同期処理である想定なので Task.Run .でラップする
        var task = Task.Run(() => item.InvokeAsync(arg));
        StateHasChanged();

        return task.IsCompleted ?
            Task.CompletedTask :
            HandleUncompletedEventAsync(task);
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

        StateHasChanged();
    }
}
