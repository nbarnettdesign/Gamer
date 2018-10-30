public enum VRDevice { Occulus, Vive }

public static class GetVRDevice
{
    public static VRDevice GetDevice(string deviceName)
    {
        if (deviceName.ToLower().Contains("oculus"))
            return VRDevice.Occulus;
        else if (deviceName.ToLower().Contains("vive"))
            return VRDevice.Vive;

        return VRDevice.Vive;
    }
}
