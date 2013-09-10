namespace Business.Interfaces
{
    public interface ISensorService
    {
        bool IsSensorSerialValid(string p, out ErrorClasses.GenericError error);

        bool EditSensorSerial(string p1, string p2, string Username, out ErrorClasses.GenericError error);
    }
}