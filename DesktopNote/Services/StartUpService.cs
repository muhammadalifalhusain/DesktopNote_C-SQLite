//using System;
//using System.Threading.Tasks;
//using Windows.ApplicationModel;

//namespace DesktopNote.Services;

//public static class StartupService
//{
//    private const string TaskId = "DesktopNoteStartup";

//    public static async Task<bool> IsEnabledAsync()
//    {
//        var startupTask =
//            await StartupTask.GetAsync(TaskId);

//        return startupTask.State ==
//               StartupTaskState.Enabled;
//    }

//    public static async Task<bool> EnableAsync()
//    {
//        var startupTask =
//            await StartupTask.GetAsync(TaskId);

//        var state =
//            await startupTask.RequestEnableAsync();

//        return state ==
//               StartupTaskState.Enabled;
//    }

//    public static async Task DisableAsync()
//    {
//        var startupTask =
//            await StartupTask.GetAsync(TaskId);

//        if (startupTask.State ==
//            StartupTaskState.Enabled)
//        {
//            startupTask.Disable();
//        }
//    }
//}