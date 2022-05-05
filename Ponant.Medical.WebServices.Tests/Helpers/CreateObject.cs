namespace Ponant.Medical.WebServices.Tests.Helpers
{
    using Ponant.Medical.Data;
    using Ponant.Medical.Data.Auth;
    using Ponant.Medical.Data.Shore;
    using Ponant.Medical.WebServices.Tests.Mocks;
    using System;
    using System.Collections.Generic;

    public class CreateObject
    {
        #region PassengerCreate
        public Passenger PassengerCreate(
            TestHttpContext testHttpContext,
            int id,
            string lastname = "Test",
            string firstname = "Test",
            string email = "Email",
            int idStatus = 0,
            int idAdvice = 0,
            bool isExtract = false)
        {
            Passenger passenger = new Passenger
            {
                Id = id,
                LastName = lastname,
                FirstName = firstname,
                Email = email,
                IdStatus = idStatus,
                IdAdvice = idAdvice,
                IsExtract = isExtract,
                Creator = testHttpContext.httpContext.User.Identity.Name,
                CreationDate = DateTime.Now,
                Editor = testHttpContext.httpContext.User.Identity.Name,
                ModificationDate = DateTime.Now,
                IsDownloaded = false
            };

            return passenger;
        }
        #endregion

        #region CruiseCreate
        public Cruise CruiseCreate(
            TestHttpContext testHttpContext,
            int id,
            int idTypeCruise,
            int idShip,
            int idDestination,
            string code,
            int sailingDateDay = 10,
            int sailingLengthDays = 10,
            bool isExtract = true
            )
        {
            List<BookingCruisePassenger> bookingCruisePassengerList = new List<BookingCruisePassenger>
            {
                new BookingCruisePassenger
                {
                    Passenger = PassengerCreate(
                        testHttpContext: testHttpContext, 
                        id: 1, 
                        idStatus: Constants.SHORE_STATUS_QM_IN_PROGRESS,
                        isExtract: true)
                },
                new BookingCruisePassenger
                {
                    Passenger = PassengerCreate(
                        testHttpContext: testHttpContext, 
                        id: 2, 
                        idStatus: Constants.SHORE_STATUS_QM_SENT,
                        isExtract: true)
                }
            };

            Cruise cruise = new Cruise
            {
                Id = id,
                IdTypeCruise = idTypeCruise,
                IdShip = idShip,
                IdDestination = idDestination,
                Code = code,
                SailingDate = DateTime.Today.AddDays(sailingDateDay),
                SailingLengthDays = sailingLengthDays,
                IsExtract = isExtract,
                Extract = null,
                CreationDate = DateTime.Now,
                Editor = "System",
                ModificationDate = DateTime.Now,
                BookingCruisePassenger = bookingCruisePassengerList
            };

            return cruise;
        }
        #endregion

        #region VCruiseBoardCreate
        public vCruiseBoard VCruiseBoardCreate(
            int id,
            string code,
            int sailingDateDay,
            int sailingLengthDay,
            int idShip)
        {
            vCruiseBoard vCruiseBoard = new vCruiseBoard
            {
                Id = id,
                Code = code,
                SailingDate = DateTime.Today.AddDays(sailingDateDay),
                SailingLengthDays = sailingLengthDay,
                NbPassenger = 23,
                IdShip = idShip
            };

            return vCruiseBoard;
        }
        #endregion

        #region CreateLov
        public Lov LovCreate(
           int id,
           int idLovType,
           string code,
           string name)
        {
            Lov lov = new Lov
            {
                Id = id,
                IdLovType = idLovType,
                Code = code,
                Name = name,
                IsEnabled = true,
                Creator = "System",
                CreationDate = DateTime.UtcNow,
                Editor = "System",
                ModificationDate = DateTime.UtcNow
            };

            return lov;
        }
        #endregion

        #region BookingCruisePassengerCreate
        public BookingCruisePassenger BookingCruisePassengerCreate(
            int idCruise,
            bool isExtract = false,
            string extract = "test")
        {
            BookingCruisePassenger bcp = new BookingCruisePassenger
            {
                IdCruise = idCruise,
                Cruise = new Cruise
                {
                    IsExtract = isExtract,
                    Extract = extract,
                    ModificationDate = DateTime.Today.AddDays(-1)
                },
            };

            return bcp;
        }
        #endregion

        #region VPassengerBoardCreate
        public vPassengerBoard VPassengerBoardCreate(
            int id,
            int idCruise,
            int idStatus)
        {
            vPassengerBoard vpb = new vPassengerBoard
            {
                Id = id,
                IdCruise = idCruise,
                IdStatus = idStatus
            };

            return vpb;
        }
        #endregion

        #region DocumentCreate
        public Document DocumentCreate(
            int id,
            int idPassenger = 0,
            string email = "test@test.com",
            string filename = "filename",
            int idStatus = 0,
            string message = "test",
            string name = "test")
        {
            Document document = new Document
            {
                Id = id,
                IdPassenger = idPassenger,
                Email = email,
                FileName = filename,
                IdStatus = idStatus,
                Message = message,
                Name = name,
                ReceiptDate = DateTime.Now,
                CreationDate = DateTime.Now,
                Creator = "test",
                ModificationDate = DateTime.Now,
                Editor = "test",
            };

            if(idPassenger != 0)
            {
                document.Passenger = new Passenger
                {
                    Id = 1
                };
            }

            return document;
        }
        #endregion

        #region AspNetUserCreate
        public AspNetUsers AspNetUserCreate(
            int idShip,
            string passwordHash = "test",
            string passwordHashInit = "test",
            string username = "test",
            string roleName = null)
        {
            AspNetUsers aspNetUser = new AspNetUsers
            {
                IdShip = idShip,
                PasswordHash = passwordHash,
                PasswordHashInit = passwordHashInit,
                UserName = username,          
            };

            if (!string.IsNullOrWhiteSpace(roleName))
            {
                aspNetUser.AspNetRoles = new List<AspNetRoles>
                {
                    new AspNetRoles
                    {
                        Name = roleName
                    }
                };
            }

            return aspNetUser;
        }
        #endregion

        #region AdviceBoardCreate
        public AdviceBoard AdviceBoardCreate(
            int idPassenger,
            int idAdvice,
            int idCruise,
            string comments = "comments")
        {
            List<int> informations = new List<int> { 1, 2, 3 };
            AdviceBoard adviceBoard = new AdviceBoard
            {
                IdPassenger = idPassenger,
                IdAdvice = idAdvice,
                IdCruise = idCruise,
                Comments = comments,
                Informations = informations
            };

            return adviceBoard;
        }
        #endregion

        #region UserBoardCreate
        public UserBoard UserBoardCreate(
            string username,
            string password)
        {
            UserBoard userBoard = new UserBoard
            {
                UserName = username,
                PasswordHash = password
            };

            return userBoard;
        }
        #endregion
    }
}
