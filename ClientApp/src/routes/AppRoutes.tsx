import React from 'react';
import { Routes, Route, Navigate } from 'react-router-dom';
import UserManagementDashboard from '../pages/User Management/Dashboard/Dashboard.tsx';
import CreateUserAccess from '../pages/User Management/Create User Access/CreateUserAcces';
import ManageRoles from '../pages/User Management/Manage Roles/ManageRoles';
import EmployeeMaintenance from '../pages/User Management/Employee Maintenance/EmployeeMaintenance';
import GroupManagementDashboard from '../pages/Group Management/Dashboard/Dashboard.tsx';
import AuditLogs from '../pages/Generate Report/Audit Logs/AuditLogs';
import UserList from '../pages/Generate Report/User List/UserList';
import GroupList from '../pages/Generate Report/Group List/GroupList';
import ExceptionManagementDashboard from '../pages/Exception Management/Dashboard/Dashboard';
import AddException from '../pages/Exception Management/AddException/AddException';
import ViewExceptionDetails from '../pages/Exception Management/ViewExceptionDetails/ViewExceptionDetails';
import EditException from '../pages/Exception Management/EditException/EditException';
import ForApprovalException from '../pages/Exception Management/For Approval (Exception)/ForApprovalException';
import ForApprovalExceptionDetails from '../pages/Exception Management/For Approval (Exception Details)/ForApprovalExceptionDetails';
import ForApprovalSubException from '../pages/Exception Management/For Approval (Sub-Exception)/ForApprovalSubException';
import SubExceptionDetails from '../pages/Exception Management/SubException/SubExceptionDetails';
import ReportManagementDashboard from '../pages/Reports Management/Dashboard/Dashboard';
import GenerateRegularReports from '../pages/Reports Management/Generate Regular Reports/GenerateRegularReports';
import BranchReply from '../pages/Reports Management/Dashboard/BranchReply';
import Login from '../pages/Authentication/Login';
import HomePage from '../pages/HomePage/HomePage.tsx';
import ReportDetails from '../pages/Reports Management/Dashboard/ReportDetails.tsx';


const AppRoutes: React.FC = () => (

  <Routes>
    <Route path="/" element={<Navigate to="/login" replace />} />
    <Route path="/login" element={<Login />} />

    {/* User Management Routes */}
    <Route path='/Homepage' element={<HomePage />} />
    <Route path="/UserManagement" element={<UserManagementDashboard />} />
    <Route path="/UserManagement/Dashboard" element={<UserManagementDashboard />} />
    <Route path="/UserManagement/CreateUserAccess" element={<CreateUserAccess />} />
    <Route path="/UserManagement/ManageRoles" element={<ManageRoles />} />
    <Route path="/UserManagement/EmployeeMaintenance" element={<EmployeeMaintenance />} />

    {/* Group Management Routes */}
    <Route path="/GroupManagement" element={<GroupManagementDashboard />} />
    <Route path="/GroupManagement/Dashboard" element={<GroupManagementDashboard />} />
    {/* <Route path="/GroupManagement/CreateGroup" element={<CreateGroup />} /> */}

    {/* Generate Report Routes */}
    <Route path="/GenerateReport" element={<AuditLogs />} />
    <Route path="/GenerateReport/AuditLogs" element={<AuditLogs />} />
    <Route path="/GenerateReport/UserList" element={<UserList />} />
    <Route path="/GenerateReport/GroupList" element={<GroupList />} />

    {/* Exceptions Management Routes */}
    <Route path="/ExceptionsManagement" element={<ExceptionManagementDashboard />} />
    <Route path="/ExceptionsManagement/Dashboard" element={<ExceptionManagementDashboard />} />
    <Route path="/ExceptionsManagement/Details/:refNo" element={<ViewExceptionDetails />} />
    <Route path="/ExceptionsManagement/Edit/:refNo" element={<EditException />} />
    <Route path="/ExceptionsManagement/AddException" element={<AddException />} />
    <Route path="/ExceptionsManagement/ForApprovalExceptions" element={<ForApprovalException />} />
    <Route path="/ExceptionsManagement/Approval/:refNo" element={<ForApprovalExceptionDetails />} />
    <Route path="/ExceptionsManagement/ForApprovalSubExceptions" element={<ForApprovalSubException />} />
    {/* SubExceptions Management Routes */}
    <Route path="/SubExceptionsManagement/Details/:subERN" element={<SubExceptionDetails />} />

    {/* Reports Management Routes */}
    <Route path="/ReportsManagement" element={<ReportManagementDashboard />} />
    <Route path="/ReportsManagement/Dashboard" element={<ReportManagementDashboard />} />
    <Route path="/ReportsManagement/GenerateRegularReports" element={<GenerateRegularReports />} />
    <Route path="/ReportsManagement/BranchReply/:id" element={<BranchReply />} />
    <Route path="/ReportsManagement/Dashboard/ReportDetails/:id" element={<ReportDetails />} />
  </Routes>

);

export default AppRoutes;
