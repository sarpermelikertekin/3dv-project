# HoloLensBoundingBox (Unity app)

This repo is based on [**HoloLensCameraStream for Unity**](https://github.com/EnoxSoftware/HoloLensCameraStream). The original repository provides tools for mapping image pixel coordinate from the HoloLens video camera to 3D coordinate.

## Getting Started
The tutorial below will show you how to **Build and deploy the Unity app** or **Install the Unity app with appxbundle file*.

### Things you need
* [The HoloLens development tools](https://developer.microsoft.com/en-us/windows/mixed-reality/install_the_tools), sans Vuforia and the Emulator.

### Build and deploy the Unity app
The example Unity project can be found in the root `HoloLensVideoCaptureExample/` directory. This Unity project is a great way to learn how to use the CameraStream plugin in Unity, or to use as a template for your own Unity project. Read on to learn how to build and run the example project on your HoloLens. You should be familiar with creating and configuring a new Unity-HoloLens project [according to Microsoft's instructions](https://developer.microsoft.com/en-us/windows/mixed-reality/holograms_100). As Microsoft and Unity update their HoloLens documentation, I'm sure this tutorial will become out of date.
1. **Open the example project:** Navigate to and open the example project directory (`HoloLensVideoCaptureExample/`) in Unity.
2. **Configure build settings:** Once the project opens, select **File > Build Settings**. In the Platform list, select `Windows Store`. Set the SDK to `Universal 10`; set Target device to `HoloLens`; set UWP Build Type to `D3D`; check the Unity C# Projects checkbox; and finally, click **Switch Platform**.
3. **Add Define Symbols:** Open **File > Build Settings > Player Settings > Other Settings** and add the following to `Scripting Define Symbols` depending on the XR system used in your project; Legacy built-in XR: `BUILTIN_XR`'; XR Plugin Management (Windows Mixed Reality): `XR_PLUGIN_WINDOWSMR`; XR Plugin Management (OpenXR):`XR_PLUGIN_OPENXR`.
4. **Build the project:** You can now build the Unity project, which generates a Visual Studio Solution (which you will then have to also build). With the Build Settings window still open, click **Build**. In the explorer window that appears, make a new folder called `App`, which should live as a sibling next to the 'Assets` folder. Then click Select Folder to generate the VS solution in that folder. Then wait for Unity to build the solution.
5. **Open the VS Solution:** When the solution is built, a Windows explorer folder will open. Open the newly-built VS solution, which lives in `App/HoloLensVideoCaptureExample.sln`. This is the solution that ultimately gets deployed to your HoloLens.
6. **Configure the deploy settings:** In the Visual Studio toolbar, change the solution platform from `ARM` to `x86` if you are building for Hololens1 or to `ARM64` if you are building for Hololens2; Change the deploy target (the green play button) to `Device` (if your HoloLens is plugged into your computer), or `Remote Machine` (if your HoloLens is connected via WiFi).
7. **Run the app:** Go to **Debug > Start Debugging**. Once the app is deployed to the HoloLens, you should see some confirmation output in the Output window.

### Install the Unity app with appxbundle file

The application is also provided as a single appxbundle file and can be installed as follow.

1. Download the [appxbundle](https://github.com/sarpermelikertekin/3dv-tcp-communication/new/main/HoloLensBoundingBox/).
2. Go to the Device Portal (type the IP address of your HoloLens in the address bar of your preferred web browser) and upload the appxbundle to the HoloLens (System -> File explorer -> Downloads).
3. On your HoloLens, open the File Explorer and locate the appxbundle. Tap the appxbundle file to open the installer and tap Install.
