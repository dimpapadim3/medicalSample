using Business.Interfaces;
using System;

namespace Business
{
    public  class Sensors : ISensorService
    {
        private  bool IsSerialValid(string serial, out ErrorClasses.GenericError error)
        {
            error = null;

            if (string.IsNullOrEmpty(serial))
                return false;
            if (serial.Length != Common.Constants.SERIAL_NUMBER_LENGTH)
            {
                error = ErrorClasses.Controller.GetInvalidSerialLength();
                return false;
            }

            serial = serial.ToUpper();

            if ((serial[0] != 'M') &&
                (serial[0] != 'R'))
            {
                error = ErrorClasses.Controller.GetInvalidSerialFormat();
                return false;
            }


            if (serial[1] != 'K')
            {
                error = ErrorClasses.Controller.GetInvalidSerialFormat();
                return false;
            }

            bool weekValid = false;
            bool yearValid = false;
            bool runningValid = false;

            try
            {
                int week = Convert.ToInt32(serial.Substring(2, 2));
                if (week <= 53)
                    weekValid = true;
            }
            catch
            {
            }

            try
            {
                int year = Convert.ToInt32(serial.Substring(4, 2)) + 2000;
                if ((year >= 2012) && (year <= DateTime.Now.Year))
                    yearValid = true;
            }
            catch
            {
            }

            try
            {
                int running = Convert.ToInt32(serial.Substring(6, 4));
                runningValid = true;
            }
            catch
            {
            }

            if (
                (!weekValid) ||
                (!yearValid) ||
                (!runningValid)
                )
            {
                error = ErrorClasses.Controller.GetInvalidSerialFormat();
                return false;
            }


            return true;
        }
        
        public  bool IsSensorSerialValid(string serial,
            out ErrorClasses.GenericError error)
        {
            error = null;

            if (!IsSerialValid(serial, out error))
                return false;
            

            try
            {
                bool serialExists = SqlDataAccess.Entities.Sensor.GetSerialExists(serial, out error);

                if (error != null)
                    return false;

                if (serialExists)
                {
                    error = ErrorClasses.Controller.GetSerialExists();
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                Logger.Controller.LogError(ex);
                error = ErrorClasses.Controller.GetUnknownError();
                return false;
            }
        }

        public  bool EditSensorSerial(string oldSerial, string newSerial, string masterUserUsername,
            out ErrorClasses.GenericError error)
        {
            error = null;

            if (string.IsNullOrEmpty(oldSerial))
            {
                error = ErrorClasses.Controller.GetMissingParameterError("Old Serial");
                return false;
            }

            if (string.IsNullOrEmpty(oldSerial))
            {
                error = ErrorClasses.Controller.GetMissingParameterError("New Serial");
                return false;
            }

            if (!IsSerialValid(oldSerial, out error))
                return false;
            if (!IsSerialValid(newSerial, out error))
                return false;

            long masterUserId = SqlDataAccess.Entities.User.GetUserId(masterUserUsername, null, out error);
            if (error != null)
                return false;
            if (masterUserId <= 0)
            {
                error = ErrorClasses.Controller.GetUserNotFoundError();
                return false;
            }
            
            try
            {
                bool res = SqlDataAccess.Entities.Sensor.UpdateSerials(oldSerial, newSerial,masterUserId,
                    out error);

                if (error != null)
                    return false;

                return true;
            }
            catch (Exception ex)
            {
                Logger.Controller.LogError(ex);
                error = ErrorClasses.Controller.GetUnknownError();
                return false;
            }
        }
    }
}
