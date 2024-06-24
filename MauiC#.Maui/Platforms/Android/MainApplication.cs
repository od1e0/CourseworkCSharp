using Android.App;
using Android.Runtime;

namespace MauiC_.Maui
{
    [Application]
    [MetaData("com.google.android.maps.v2.API_KEY",
            Value = "AIzaSyDYqPTx-Uu6QTcfedkvxx1owf6uI8g8JgY")]
    public class MainApplication : MauiApplication
    {
        public MainApplication(IntPtr handle, JniHandleOwnership ownership)
            : base(handle, ownership)
        {
        }

        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
    }
}
