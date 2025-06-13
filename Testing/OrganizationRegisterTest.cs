using ACCI_Center.Service.RegisterInformation;
using ACCI_Center.Dto.Request;
using ACCI_Center.BusinessResult;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using ACCI_Center.Dao.ExamSchedule;
using ACCI_Center.Dao.Invoice;
using ACCI_Center.Dao.RegisterInformation;

namespace Testing
{
    [TestClass]
    public class OrganizationRegisterTest
    {
        private Mock<IRegisterInformationDao> _registerInformationDaoMock;
        private Mock<IExamScheduleDao> _examScheduleDaoMock;
        private Mock<IInvoiceDao> _invoiceDaoMock;
        private OrganizationRegisterInformationService _service;

        [TestInitialize]
        public void Setup()
        {
            _registerInformationDaoMock = new Mock<IRegisterInformationDao>();
            _examScheduleDaoMock = new Mock<IExamScheduleDao>();
            _invoiceDaoMock = new Mock<IInvoiceDao>();
            _service = new OrganizationRegisterInformationService(
                _registerInformationDaoMock.Object,
                _examScheduleDaoMock.Object,
                _invoiceDaoMock.Object
            );
        }

        private OrganizationRegisterRequest GetValidRequest()
        {
            return new OrganizationRegisterRequest
            {
                registerInformation = new ACCI_Center.Entity.RegisterInformation
                {
                    HoTen = "Org Name",
                    SDT = "0123456789",
                    Email = "org@email.com",
                    DiaChi = "Address"
                },
                testId = 1,
                testName = "TOEIC",
                desiredExamTime = DateTime.Now.AddDays(1),
                candidateInformations = new List<ACCI_Center.Entity.CandidateInformation>
                {
                    new ACCI_Center.Entity.CandidateInformation { HoTen = "A", SDT = "0123456789", Email = "a@gmail.com" }
                }
            };
        }

        [TestMethod]
        public void RegisterForOrganization_ReturnsSuccess_WhenAllValid()
        {
            var request = GetValidRequest();

            _examScheduleDaoMock.Setup(x => x.GetTestById(request.testId))
                .Returns(new ACCI_Center.Entity.Test { MaBaiThi = request.testId, TenBaiThi = request.testName });
            _examScheduleDaoMock.Setup(x => x.GetAllEmptyRoomIds(request.desiredExamTime, request.testId))
                .Returns(new List<int> { 1 });
            _examScheduleDaoMock.Setup(x => x.GetAllFreeEmployeeIds(request.desiredExamTime, request.testId))
                .Returns(new List<int> { 1 });
            _examScheduleDaoMock.Setup(x => x.AddExamSchedule(It.IsAny<ACCI_Center.Entity.ExamSchedule>(), It.IsAny<int>()))
                .Returns(1);
            _registerInformationDaoMock.Setup(x => x.AddRegisterInformation(It.IsAny<ACCI_Center.Entity.RegisterInformation>()))
                .Returns(1);
            _registerInformationDaoMock.Setup(x => x.AddCandidateInformationsOfARegisterInformation(It.IsAny<int>(), It.IsAny<List<ACCI_Center.Entity.CandidateInformation>>()))
                .Returns(1);
            _invoiceDaoMock.Setup(x => x.AddInvoice(It.IsAny<ACCI_Center.Entity.Invoice>()))
                .Returns(1);
            _examScheduleDaoMock.Setup(x => x.GetFeeOfTheTest(request.testId)).Returns(100);

            var result = _service.RegisterForOrganization(request);

            Assert.AreEqual(RegisterResult.Success, result);
        }

        [TestMethod]
        public void RegisterForOrganization_ReturnsInvalidOrganizationInformation_WhenOrgInfoInvalid()
        {
            var request = GetValidRequest();
            request.registerInformation.HoTen = "";

            var result = _service.RegisterForOrganization(request);

            Assert.AreEqual(RegisterResult.InvalidOrganizationInformation, result);
        }

        [TestMethod]
        public void RegisterForOrganization_ReturnsInvalidTestInformation_WhenTestInfoInvalid()
        {
            var request = GetValidRequest();

            _examScheduleDaoMock.Setup(x => x.GetTestById(request.testId))
                .Returns((ACCI_Center.Entity.Test)null);

            var result = _service.RegisterForOrganization(request);

            Assert.AreEqual(RegisterResult.InvalidTestInformation, result);
        }

        [TestMethod]
        public void RegisterForOrganization_ReturnsNoAvailableTimeSlot_WhenNoRoomOrEmployee()
        {
            var request = GetValidRequest();

            _examScheduleDaoMock.Setup(x => x.GetTestById(request.testId))
                .Returns(new ACCI_Center.Entity.Test { MaBaiThi = request.testId, TenBaiThi = request.testName });
            _examScheduleDaoMock.Setup(x => x.GetAllEmptyRoomIds(request.desiredExamTime, request.testId))
                .Returns(new List<int>());
            _examScheduleDaoMock.Setup(x => x.GetAllFreeEmployeeIds(request.desiredExamTime, request.testId))
                .Returns(new List<int>());

            var result = _service.RegisterForOrganization(request);

            Assert.AreEqual(RegisterResult.NoAvailableTimeSlot, result);
        }

        [TestMethod]
        public void RegisterForOrganization_ReturnsUnknownError_WhenAddExamScheduleFails()
        {
            var request = GetValidRequest();

            _examScheduleDaoMock.Setup(x => x.GetTestById(request.testId))
                .Returns(new ACCI_Center.Entity.Test { MaBaiThi = request.testId, TenBaiThi = request.testName });
            _examScheduleDaoMock.Setup(x => x.GetAllEmptyRoomIds(request.desiredExamTime, request.testId))
                .Returns(new List<int> { 1 });
            _examScheduleDaoMock.Setup(x => x.GetAllFreeEmployeeIds(request.desiredExamTime, request.testId))
                .Returns(new List<int> { 1 });
            _examScheduleDaoMock.Setup(x => x.AddExamSchedule(It.IsAny<ACCI_Center.Entity.ExamSchedule>(), It.IsAny<int>()))
                .Returns(-1);

            var result = _service.RegisterForOrganization(request);

            Assert.AreEqual(RegisterResult.UnknownError, result);
        }

        [TestMethod]
        public void RegisterForOrganization_ReturnsUnknownError_WhenAddRegisterInformationFails()
        {
            var request = GetValidRequest();

            _examScheduleDaoMock.Setup(x => x.GetTestById(request.testId))
                .Returns(new ACCI_Center.Entity.Test { MaBaiThi = request.testId, TenBaiThi = request.testName });
            _examScheduleDaoMock.Setup(x => x.GetAllEmptyRoomIds(request.desiredExamTime, request.testId))
                .Returns(new List<int> { 1 });
            _examScheduleDaoMock.Setup(x => x.GetAllFreeEmployeeIds(request.desiredExamTime, request.testId))
                .Returns(new List<int> { 1 });
            _examScheduleDaoMock.Setup(x => x.AddExamSchedule(It.IsAny<ACCI_Center.Entity.ExamSchedule>(), It.IsAny<int>()))
                .Returns(1);
            _registerInformationDaoMock.Setup(x => x.AddRegisterInformation(It.IsAny<ACCI_Center.Entity.RegisterInformation>()))
                .Returns(-1);

            var result = _service.RegisterForOrganization(request);

            Assert.AreEqual(RegisterResult.UnknownError, result);
        }

        [TestMethod]
        public void RegisterForOrganization_ReturnsUnknownError_WhenAddCandidateInformationsFails()
        {
            var request = GetValidRequest();

            _examScheduleDaoMock.Setup(x => x.GetTestById(request.testId))
                .Returns(new ACCI_Center.Entity.Test { MaBaiThi = request.testId, TenBaiThi = request.testName });
            _examScheduleDaoMock.Setup(x => x.GetAllEmptyRoomIds(request.desiredExamTime, request.testId))
                .Returns(new List<int> { 1 });
            _examScheduleDaoMock.Setup(x => x.GetAllFreeEmployeeIds(request.desiredExamTime, request.testId))
                .Returns(new List<int> { 1 });
            _examScheduleDaoMock.Setup(x => x.AddExamSchedule(It.IsAny<ACCI_Center.Entity.ExamSchedule>(), It.IsAny<int>()))
                .Returns(1);
            _registerInformationDaoMock.Setup(x => x.AddRegisterInformation(It.IsAny<ACCI_Center.Entity.RegisterInformation>()))
                .Returns(1);
            _registerInformationDaoMock.Setup(x => x.AddCandidateInformationsOfARegisterInformation(It.IsAny<int>(), It.IsAny<List<ACCI_Center.Entity.CandidateInformation>>()))
                .Returns(-1);

            var result = _service.RegisterForOrganization(request);

            Assert.AreEqual(RegisterResult.UnknownError, result);
        }

        [TestMethod]
        public void RegisterForOrganization_ReturnsUnknownError_WhenAddInvoiceFails()
        {
            var request = GetValidRequest();

            _examScheduleDaoMock.Setup(x => x.GetTestById(request.testId))
                .Returns(new ACCI_Center.Entity.Test { MaBaiThi = request.testId, TenBaiThi = request.testName });
            _examScheduleDaoMock.Setup(x => x.GetAllEmptyRoomIds(request.desiredExamTime, request.testId))
                .Returns(new List<int> { 1 });
            _examScheduleDaoMock.Setup(x => x.GetAllFreeEmployeeIds(request.desiredExamTime, request.testId))
                .Returns(new List<int> { 1 });
            _examScheduleDaoMock.Setup(x => x.AddExamSchedule(It.IsAny<ACCI_Center.Entity.ExamSchedule>(), It.IsAny<int>()))
                .Returns(1);
            _registerInformationDaoMock.Setup(x => x.AddRegisterInformation(It.IsAny<ACCI_Center.Entity.RegisterInformation>()))
                .Returns(1);
            _registerInformationDaoMock.Setup(x => x.AddCandidateInformationsOfARegisterInformation(It.IsAny<int>(), It.IsAny<List<ACCI_Center.Entity.CandidateInformation>>()))
                .Returns(1);
            _invoiceDaoMock.Setup(x => x.AddInvoice(It.IsAny<ACCI_Center.Entity.Invoice>()))
                .Returns(-1);

            var result = _service.RegisterForOrganization(request);

            Assert.AreEqual(RegisterResult.UnknownError, result);
        }
    }
}
