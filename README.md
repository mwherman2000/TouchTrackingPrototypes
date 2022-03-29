# TouchTrackingPrototypes
Xamarin.Forms Touch Tracking Prototypes using PlatformEffect

## Version 0: TouchTrackingPrototype0

- Uses the `Blank` Xamarin Forms template in Visual Studio 2022 (third option) to create a simple single page XAML app.
- The content of the `MainPage.xaml` was updated to render a simple `Grid` element inside a colored `Frame` element.
- Nothing more

```xml
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://xamarin.com/schemas/2014/forms"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             x:Class="TouchTrackingPrototype0.MainPage">

    <Frame BackgroundColor="#2196F3" Padding="24" CornerRadius="0">
        <Grid>
            <Label Text="Label inside Grid inside a Frame" FontSize="Title" TextColor="White" Padding="30,10,30,10"/>
        </Grid>
    </Frame>
</ContentPage>
```

## Version 1: TouchTrackingPrototype1

- Extends `TouchTrackingPrototype0` to include a `PlatformEffect` class (element `pe:TouchTrackingEffect`) that does nothing more than activate and deactivate (attach and detach).
- `MainPage.xaml` changes: added namespace `pe`; then `pe:TouchTrackingEffect` element to the page markup

```xml
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://xamarin.com/schemas/2014/forms"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:pe="clr-namespace:TouchTrackingPlatformEffects"
             x:Class="TouchTrackingPrototype1.MainPage">

    <Frame BackgroundColor="#2196F3" Padding="24" CornerRadius="0">
        <Grid>
            <Label Text="Label inside Grid inside a Frame" FontSize="Title" TextColor="White" Padding="30,10,30,10"/>

            <Grid.Effects>
                <pe:TouchTrackingEffect />
            </Grid.Effects>
        </Grid>
    </Frame>
</ContentPage>
```

- Added `TouchTrackingEffect.cs` to the device-independent (top-most) project of the solution. This class implements the _code-behind_ the `pe:TouchTrackingEffect` element. Through magic, the `TouchTrackingEffect()` method binds to either the Android or iOS platform-specific implementations of this class. This class derives from `RoutingEffect`.

```csharp
using Xamarin.Forms;

namespace TouchTrackingPlatformEffects
{
    public class TouchTrackingEffect : RoutingEffect
    {
        public TouchTrackingEffect() : base("XamarinDocs.TouchTrackingEffect")
        {
            System.Diagnostics.Debug.WriteLine("TouchTrackingEffect():entered");
        }
    }
}

```

- Added `TouchTrackingEffectDroid.cs` to the Android (second) project in the solution. This class is where the Android specific _platform-specific effect_ (or _platform-specific behavior_) is implemented.
- When a XAML page is _pushed_ on the app stack to become the currently displayed page, the `OnAttached()` method is called to perform any device-specific (Android) initialization; for example, configure an event handler or dynamically change the color or style of a control.
- When the XAML page is _popped_ off the stack (and the previous page is displayed), the `OnDetached()` method is called to undo any initializations performed in `OnAttached()` (e.g. remove an event handler).
- These 2 methods don't do anything in this version. The goal of this version is simply to connect up all the basic plumbing and to test that it works.
- It's useful to set breakpoints on each of the `WriteLine()` calls.

```csharp
using System;
using System.Collections.Generic;
using System.Linq;

using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

using Android.Views;

[assembly: ResolutionGroupName("XamarinDocs")]
[assembly: ExportEffect(typeof(TouchTrackingPlatformEffects.Droid.TouchTrackingEffect), "TouchTrackingEffect")]

namespace TouchTrackingPlatformEffects.Droid
{
    public class TouchTrackingEffect : PlatformEffect
    {
        protected override void OnAttached()
        {
            System.Diagnostics.Debug.WriteLine("TouchTrackingEffect.OnAttached():entered");
        }

        protected override void OnDetached()
        {
            System.Diagnostics.Debug.WriteLine("TouchTrackingEffect.OnDetached():entered");
        }
    }
}

```

## Version 2: TouchTrackingPrototype2

- This version can be skipped. See the NOTE: below.
- This version only adds a `Button` (and event handler) to `MainPage.xaml` to push to a `DonePage.xaml`. It illustrates how _pushing_ activates the `OnAttached()` handler but the page push to `DonePage.xaml` doesn't activate the `OnDetached()` handler. Checkout Version 3.
- NOTE: There is one important change in Version 2 needed to get _Navigation_ to work: In the `App()` constructor, the statement `MainPage = new MainPage();` needs to be changed to `MainPage = new NavigationPage(new MainPage());`

## Version 3: TouchTrackingPrototype3

- Extends `TouchTrackingPrototype2` to demonstate both `OnAttached()` and `OnDetached()` being executed. The previous versions were only able to cause `OnAttached()` to be executed. It took some additional research (trial-and-error) to figure out that a page needed to be _popped_ to trigger `OnDetached()`. Here's what needed to be changed...
- Removed `DonePage.xaml`
- Added `TouchTrackerPage.xaml`. All of the touch tracking was moved from `MainPage.xaml` to `TouchTrackerPage.xaml`. `TouchTrackerPage.xaml` is the page that is pushed from `MainPage.xaml` (and later popped via an event handler attached to a `Button`). This combination results in `OnAttached()` to be executed when `TouchTrackerPage.xaml` is pushed from `MainPage.xaml` and, for the first time, `OnDetached()` to be executed (when `TouchTrackerPage.xaml` is popped).

```xml
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://xamarin.com/schemas/2014/forms"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:pe="clr-namespace:TouchTrackingPlatformEffects"
             xmlns:pages="clr-namespace:TouchTrackingPrototype3"
             x:Class="TouchTrackingPrototype3.TouchTrackerPage">

    <Frame BackgroundColor="#2196F3" Padding="24" CornerRadius="0">
        <Grid>
            <Grid.RowDefinitions>
                <RowDefinition Height="Auto" />
                <RowDefinition Height="Auto" />
                <RowDefinition Height="*" />
            </Grid.RowDefinitions>

            <Label Text="TouchTrackerPage: Click button when done..." FontSize="Title" TextColor="White" Padding="30,10,30,10" Grid.Row="0"/>

            <Button Text="Click when done..." Clicked="DoneButton_Clicked" Grid.Row="1"/>

            <Grid.Effects>
                <pe:TouchTrackingEffect />
            </Grid.Effects>
        </Grid>
    </Frame>
</ContentPage>
```
- `TouchTrackerPage.xaml.cs` `Button` event handler that _pops_ the page thereby causing `OnDetached()` to be executed.
```csharp
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace TouchTrackingPrototype3
{
    public partial class TouchTrackerPage : ContentPage
    {
        public TouchTrackerPage()
        {
            InitializeComponent();
        }
        private async void DoneButton_Clicked(object sender, EventArgs e)
        {
            //await Navigation.PushAsync(new DonePage());
            await Navigation.PopAsync();
        }
    }
}
```
- `MainPage.xaml` with the `PlatformEffect` _removed_.
```xml
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://xamarin.com/schemas/2014/forms"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:pe="clr-namespace:TouchTrackingPlatformEffects"
             xmlns:pages="clr-namespace:TouchTrackingPrototype3"
             x:Class="TouchTrackingPrototype3.MainPage">

    <Frame BackgroundColor="#2196F3" Padding="24" CornerRadius="0">
        <Grid>
            <Grid.RowDefinitions>
                <RowDefinition Height="Auto" />
                <RowDefinition Height="Auto" />
                <RowDefinition Height="*" />
            </Grid.RowDefinitions>

            <Label Text="MainPage: Click button to start tracking..." FontSize="Title" TextColor="White" Padding="30,10,30,10" Grid.Row="0"/>

            <Button Text="Click to track..." Clicked="TrackButton_Clicked" Grid.Row="1"/>
        </Grid>
    </Frame>
</ContentPage>
```
- Lastly, `TouchTrackingEffectDroid.cs` was updated to attach the touch tracking event handler `OnTouchTrackingHandler()` for the first time. This is the first _complete_ example showing the whole sample from end-to-end.

```csharp
using System;
using System.Collections.Generic;
using System.Linq;

using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

using Android.Views;

[assembly: ResolutionGroupName("XamarinDocs")]
[assembly: ExportEffect(typeof(TouchTrackingPlatformEffects.Droid.TouchTrackingEffect), "TouchTrackingEffect")]

namespace TouchTrackingPlatformEffects.Droid
{
    public class TouchTrackingEffect : PlatformEffect
    {
        Android.Views.View view;

        protected override void OnAttached()
        {
            System.Diagnostics.Debug.WriteLine("TouchTrackingEffect.OnAttached():entered");

            view = Control == null ? Container : Control;
            if (view != null)
            {
                view.Touch += OnTouchTrackingHandler;
            }

        }

        protected override void OnDetached()
        {
            System.Diagnostics.Debug.WriteLine("TouchTrackingEffect.OnDetached():entered");

            if (view != null)
            {
                view.Touch -= OnTouchTrackingHandler;
            }
        }

        void OnTouchTrackingHandler(object sender, Android.Views.View.TouchEventArgs args)
        {
            Android.Views.View senderView = sender as Android.Views.View;
            MotionEvent motionEvent = args.Event;

            System.Diagnostics.Debug.WriteLine("OnTouchTrackingHandler: " + args.Event.Action.ToString() + ": "
                + args.Event.ActionMasked.ToString() + ": "
                + motionEvent.Pressure.ToString());
        }
    }
}
```

## Version 4: TouchTrackingPrototype4

- Exact copy of `TouchTrackingPrototype3` with some added event-level debug tracing

## Version 5: TouchTrackingPrototype5

- Exact copy of `TouchTrackingPrototype4` with the addition of the NFE envelope and proof JSON templates as _raw_ Resources in the Xamarin Android project. This includes changes in the Android platform project to save a handle to the `MainActivity` for use in the Adroid platform specific project as well as code on the `OnAttached` handler. Then, of course, there is the addition of the `raw` folder in the Android project as welll as the 2 JSON template files.
- Changes
    1. https://github.com/mwherman2000/TouchTrackingPrototypes/blob/main/TouchTrackingPrototype5/TouchTrackingPrototype3/TouchTrackingPrototype3.Android/MainActivity.cs#L13
    2. https://github.com/mwherman2000/TouchTrackingPrototypes/blob/main/TouchTrackingPrototype5/TouchTrackingPrototype3/TouchTrackingPrototype3.Android/MainActivity.cs#L21
    3. https://github.com/mwherman2000/TouchTrackingPrototypes/blob/main/TouchTrackingPrototype5/TouchTrackingPrototype3/TouchTrackingPrototype3.Android/TouchTrackingEffectDroid.cs#L39
    4. https://github.com/mwherman2000/TouchTrackingPrototypes/blob/main/TouchTrackingPrototype5/TouchTrackingPrototype3/TouchTrackingPrototype3.Android/TouchTrackingEffectDroid.cs#L53
    5. https://github.com/mwherman2000/TouchTrackingPrototypes/tree/main/TouchTrackingPrototype5/TouchTrackingPrototype3/TouchTrackingPrototype3.Android/Resources/raw - 2 files.  In the Properties, these 2 JSON template files need to be tagged as `AndroidResources`.

- Be sure to compleletly rebuild the solution.
- Late Addition: Added the (mostly commented out) `FillTemplate()` and `SaveNfe()` methods to Prototype5.  `SaveNfe()` works and saves a copy of the (currently unfilled) JSON template to a file called `kissnfe.json` in your Android `Downloads` folder. This should have been in a new version (separate from Prototype5 but it wasn't).
- Changes
    1. Added https://github.com/mwherman2000/TouchTrackingPrototypes/blob/main/TouchTrackingPrototype5/TouchTrackingPrototype3/TouchTrackingPrototype3.Android/BTTN4KNFEFactory.cs
    2. Added https://github.com/mwherman2000/TouchTrackingPrototypes/blob/main/TouchTrackingPrototype5/TouchTrackingPrototype3/TouchTrackingPrototype3.Android/BTTN4KNFEFactoryHelpers.cs
    3. `android:requestLegacyExternalStorage="true"` added to https://github.com/mwherman2000/TouchTrackingPrototypes/blob/main/TouchTrackingPrototype5/TouchTrackingPrototype3/TouchTrackingPrototype3.Android/Properties/AndroidManifest.xml to enabled `Android.OS.Environment.ExternalStorageDirectory.AbsolutePath` to be called - couldn't figure out the modern way of doing this: https://github.com/mwherman2000/TouchTrackingPrototypes/blob/main/TouchTrackingPrototype5/TouchTrackingPrototype3/TouchTrackingPrototype3.Android/BTTN4KNFEFactory.cs#L324. Some permissions were also added to this file but in the more recent versions of Android, they don't have any effect.  These permissions need to be requested programmatically: https://github.com/mwherman2000/TouchTrackingPrototypes/blob/main/TouchTrackingPrototype5/TouchTrackingPrototype3/TouchTrackingPrototype3.Android/BTTN4KNFEFactory.cs#L321

## Version 6 - TouchTrackingPrototype6
- `SaveNfe()` saves a copy of the template(s) filled with default values; that is, all of the %name% substitutions are uncommented in this version and the JSON file looks complete and valid ...even if it is only default values. NFE file pathname saved in a global variable.
- Changes
    1. https://github.com/mwherman2000/TouchTrackingPrototypes/blob/main/TouchTrackingPrototype6/TouchTrackingPrototype3/TouchTrackingPrototype3.Android/BTTN4KNFEFactory.cs#L221
    2. https://github.com/mwherman2000/TouchTrackingPrototypes/blob/main/TouchTrackingPrototype6/TouchTrackingPrototype3/TouchTrackingPrototype3.Android/BTTN4KNFEFactory.cs#L98
    3. https://github.com/mwherman2000/TouchTrackingPrototypes/blob/main/TouchTrackingPrototype6/TouchTrackingPrototype3/TouchTrackingPrototype3.Android/BTTN4KNFEFactory.cs#L344

## Version 7 - TouchTrackingPrototype7
- To support the actual tracking and recording of the 5 phases of a kiss, many changes to 
`OnAttached()`, `OnAttached()`,
`OnTouchTrackingHandler()`, and the 2 JSON template files. A lot of renaming of the curve array variable names, etc.
- Changes
    1. https://github.com/mwherman2000/TouchTrackingPrototypes/blob/main/TouchTrackingPrototype7/TouchTrackingPrototype3/TouchTrackingPrototype3.Android/TouchTrackingEffectDroid.cs
    2. https://github.com/mwherman2000/TouchTrackingPrototypes/blob/main/TouchTrackingPrototype7/TouchTrackingPrototype3/TouchTrackingPrototype3.Android/BTTN4KNFEFactory.cs
    3. https://github.com/mwherman2000/TouchTrackingPrototypes/blob/main/TouchTrackingPrototype7/TouchTrackingPrototype3/TouchTrackingPrototype3.Android/Resources/raw/envelope4.json
    4. https://github.com/mwherman2000/TouchTrackingPrototypes/blob/main/TouchTrackingPrototype7/TouchTrackingPrototype3/TouchTrackingPrototype3.Android/Resources/raw/proof4.json

## Version 8 - TouchTrackingPrototype8 
- Introduction of an 'HTTPClient()' to post the filled JSON NFE file to a Trinity GraphEngine defined REST endpoint.
- New `PostNfe()` method called from `OnDetached()`.
- Changes
    1. 
    2. 

That's all folks!
