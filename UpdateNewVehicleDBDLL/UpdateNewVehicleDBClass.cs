/* Title:           Update New Vehicle DB Class
 * Date:            5-23-17
 * Author:          Terry Holmes */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NewEventLogDLL;
using NewVehicleDLL;

namespace UpdateNewVehicleDBDLL
{
    public class UpdateNewVehicleDBClass
    {
        //setting up the class
        EventLogClass TheEventLogClass = new EventLogClass();
        VehicleClass TheVehicleClass = new VehicleClass();

        //setting up the data
        VehiclesDataSet TheVehiclesDataSet = new VehiclesDataSet();
        
        OldVehiclesDataSet aOldVehiclesDataSet;
        OldVehiclesDataSet TheOldVehiclesDataSet;
        OldVehiclesDataSetTableAdapters.vehiclesTableAdapter aOldVehiclesTableAdapter;
   
        //setting up global varibles
        int gintVehicleCounter;
        int gintVehicleUpperLimit;

        public bool UpdateNewTable()
        {
            bool blnFatalError = false;
            int intCounter;
            int intNumberOfRecords;
            int intVehicleCounter;
            bool blnItemNotFound = false;
            int intVehicleID;
            bool blnActive;
            bool blnAvailable;
            string strNotes;
            DateTime datOilChangeDate;
            int intOilChangeOdometer;

            try
            {
                TheOldVehiclesDataSet = GetOldVehiclesInfo();

                TheVehiclesDataSet = TheVehicleClass.GetVehiclesInfo();

                gintVehicleUpperLimit = TheVehiclesDataSet.vehicles.Rows.Count - 1;

                if (gintVehicleUpperLimit > 0)
                {
                    gintVehicleCounter = gintVehicleUpperLimit + 1;
                }
                else
                {
                    gintVehicleCounter = 0;
                    gintVehicleUpperLimit = 0;
                }

                intNumberOfRecords = TheOldVehiclesDataSet.vehicles.Rows.Count - 1;

                for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    blnItemNotFound = true;
                    intVehicleID = TheOldVehiclesDataSet.vehicles[intCounter].VehicleID;
                    if (TheOldVehiclesDataSet.vehicles[intCounter].Active == "YES")
                    {
                        blnActive = true;
                    }
                    else
                    {
                        blnActive = false;
                    }
                    if (TheOldVehiclesDataSet.vehicles[intCounter].Available == "YES")
                    {
                        blnAvailable = true;
                    }
                    else
                    {
                        blnAvailable = false;
                    }

                    if (TheOldVehiclesDataSet.vehicles[intCounter].IsNotesNull() == true)
                    {
                        strNotes = "NOT PROVIDED";
                    }
                    else
                    {
                        strNotes = TheOldVehiclesDataSet.vehicles[intCounter].Notes;
                    }

                    if (TheOldVehiclesDataSet.vehicles[intCounter].IsLastOilChangeDateNull() == true)
                    {
                        datOilChangeDate = DateTime.Now;
                    }
                    else
                    {
                        datOilChangeDate = TheOldVehiclesDataSet.vehicles[intCounter].LastOilChangeDate;
                    }
                    if (TheOldVehiclesDataSet.vehicles[intCounter].IsLastOilChangeOdometerNull() == true)
                    {
                        intOilChangeOdometer = 0;
                    }
                    else
                    {
                        intOilChangeOdometer = TheOldVehiclesDataSet.vehicles[intCounter].LastOilChangeOdometer;
                    }

                    //checking to see if the vehicle is new system
                    if (gintVehicleCounter > 0)
                    {
                        for (intVehicleCounter = 0; intVehicleCounter <= gintVehicleUpperLimit; intVehicleCounter++)
                        {
                            if (intVehicleID == TheVehiclesDataSet.vehicles[intVehicleCounter].VehicleID)
                            {
                                blnItemNotFound = false;
                            }
                        }
                    }

                    if (blnItemNotFound == true)
                    {
                        VehiclesDataSet.vehiclesRow NewVehicleRow = TheVehiclesDataSet.vehicles.NewvehiclesRow();

                        NewVehicleRow.Active = blnActive;
                        NewVehicleRow.Available = blnAvailable;
                        NewVehicleRow.AssignedOffice = TheOldVehiclesDataSet.vehicles[intCounter].HomeOffice;
                        NewVehicleRow.BJCNumber = TheOldVehiclesDataSet.vehicles[intCounter].BJCNumber;
                        NewVehicleRow.EmployeeID = TheOldVehiclesDataSet.vehicles[intCounter].EmployeeID;
                        NewVehicleRow.LicensePlate = TheOldVehiclesDataSet.vehicles[intCounter].LicencePlate;
                        NewVehicleRow.Notes = strNotes;
                        NewVehicleRow.OilChangeDate = datOilChangeDate;
                        NewVehicleRow.OilChangeOdometer = intOilChangeOdometer;
                        NewVehicleRow.VehicleID = TheOldVehiclesDataSet.vehicles[intCounter].VehicleID;
                        NewVehicleRow.VehicleMake = TheOldVehiclesDataSet.vehicles[intCounter].Make;
                        NewVehicleRow.VehicleModel = TheOldVehiclesDataSet.vehicles[intCounter].Model;
                        NewVehicleRow.VehicleYear = Convert.ToInt32(TheOldVehiclesDataSet.vehicles[intCounter].Year);
                        NewVehicleRow.VINNumber = "NOT PROVIDED";
                        
                        TheVehiclesDataSet.vehicles.Rows.Add(NewVehicleRow);
                        TheVehicleClass.UpdateVehiclesDB(TheVehiclesDataSet);
                        gintVehicleUpperLimit = gintVehicleCounter;
                        gintVehicleCounter++;
                        TheVehicleClass.CreateVehicleID();

                    }
                }

            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Update New Vehicle DB Class // Update New Table " + Ex.ToString());

                blnFatalError = true;
            }

            return blnFatalError;
        }
        public OldVehiclesDataSet GetOldVehiclesInfo()
        {
            try
            {
                aOldVehiclesDataSet = new OldVehiclesDataSet();
                aOldVehiclesTableAdapter = new OldVehiclesDataSetTableAdapters.vehiclesTableAdapter();
                aOldVehiclesTableAdapter.Fill(aOldVehiclesDataSet.vehicles);
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Update New Vehicles DB Class // Get Old Vehicles Info " + Ex.Message);
            }

            return aOldVehiclesDataSet;
        }
    }
}
