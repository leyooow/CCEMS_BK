/* eslint-disable react-hooks/exhaustive-deps */
/* eslint-disable @typescript-eslint/no-unused-vars */
import React, { useEffect, useState } from 'react';
import { Box, Card, createTheme } from '@mui/material';
import { AppProvider, type Session, type Navigation, NavigationItem, NavigationPageItem } from '@toolpad/core/AppProvider';
import { DashboardLayout } from '@toolpad/core/DashboardLayout';
import DashboardIcon from '@mui/icons-material/Dashboard';
import ManageAccounts from '@mui/icons-material/ManageAccounts';
import ManageAccountsTwoTone from '@mui/icons-material/ManageAccountsTwoTone';
import BarChartIcon from '@mui/icons-material/BarChart';
import DocumentScannerOutlined from '@mui/icons-material/DocumentScannerOutlined';
import DocumentScannerTwoTone from '@mui/icons-material/DocumentScannerTwoTone';
import ApprovalSharp from '@mui/icons-material/ApprovalSharp';
import ApprovalTwoTone from '@mui/icons-material/ApprovalTwoTone';
import { jwtDecode } from 'jwt-decode';
import './MainLayout.css';
import moment from 'moment';
import ConfirmationModal from '../components/Modal/ConfirmationModal';
import { CustomJwtPayload } from '../utils/constants';
import Cookies from 'js-cookie';
import { decrypt } from '../utils/encrypt-decrypt';
import { toast } from 'react-toastify';
import authService from '../services/authService';

let userPermissions: string[] = [];
try {
    userPermissions = JSON.parse(Cookies.get("Permission") || "[]");
} catch (error) {
    console.error("Invalid permissions:", error);
    userPermissions = [];
}


const hasChildren = (item: NavigationItem): item is NavigationPageItem & { children: Navigation } =>
    Array.isArray((item as any).children);

const filteredNavigation = (navItems: Navigation, userPermissions: string[]): Navigation =>
    navItems

        .filter(item => {
            if ('permission' in item && item.permission) {
                return item.permission.some(permission => userPermissions.includes(permission));
            }
            return true;
        })
        .map(item => {
            if (hasChildren(item)) {
                const filteredChildren = filteredNavigation(item.children, userPermissions);
                return {
                    ...item,
                    children: filteredChildren,
                };
            }
            return item;
        })
        .filter(item => {
            if (item.kind === 'header') {
                return navItems.some(
                    child =>
                        hasChildren(child) &&
                        filteredNavigation(child.children, userPermissions).length > 0
                );
            }

            if (hasChildren(item)) {
                return item.children.length > 0;
            }

            return true;
        });


const NAVIGATION: Navigation = [
    { kind: 'header', title: 'USERS', permission: ['Permissions.Users.ManageUsers']},
    {
        segment: 'UserManagement',
        title: 'User Management',
        icon: <ManageAccounts />,
        permission: ['Permissions.Users.ManageUsers'],
        children: [
            { segment: 'Dashboard', title: 'Dashboard', icon: <DashboardIcon /> },
            { segment: 'ManageRoles', title: 'Manage Roles', icon: <ManageAccounts/>,permission: ['Permissions.Users.ManagePermissions'] },
            { segment: 'EmployeeMaintenance', title: 'Employee Maintenance', icon: <ManageAccountsTwoTone /> },
        ],
    },
    { kind: 'divider', permission: ['Permissions.Users.ManageUsers'] },
    { kind: 'header', title: 'GROUPS',permission: ['Permissions.Users.ManageUsers'] },
    {
        segment: 'GroupManagement',
        title: 'Groups Management',
        icon: <BarChartIcon />,
        permission: ['Permissions.Users.ManageUsers'],
        children: [
            { segment: 'Dashboard', title: 'Dashboard', icon: <DashboardIcon /> ,permission: ['Permissions.Users.ManageUsers']},
        ],
    },

    { kind: 'divider', permission: ['Permissions.Exceptions.View'] },
    { kind: 'header', title: 'EXCEPTIONS', permission: ['Permissions.Exceptions.View']  },
    {
        segment: 'ExceptionsManagement',
        title: 'Exceptions Management',
        icon: <ApprovalTwoTone />,
        permission: ['Permissions.Exceptions.View'],
        children: [
            { segment: 'Dashboard', title: 'Dashboard', icon: <DashboardIcon />,permission: ['Permissions.Exceptions.View'] },
            { segment: 'ForApprovalExceptions', title: 'For Approval (Exceptions)', icon: <ApprovalSharp /> , permission: ['Permissions.Exceptions.Approval'] },
            { segment: 'ForApprovalSubExceptions', title: 'For Approval (Sub-Exceptions)', icon: <ApprovalSharp />, permission: ['Permissions.SubExceptions.Approval']  },
        ],
    },
    { kind: 'divider', permission: ['Permissions.Reports.View']},
    { kind: 'header', title: 'REPORTS' },
    {
        segment: 'ReportsManagement',
        title: 'Reports Management',
        icon: <DocumentScannerTwoTone />,
        permission: ['Permissions.Reports.View'],
        children: [
            { segment: 'Dashboard', title: 'Dashboard', icon: <DashboardIcon />, permission: ['Permissions.Reports.View'] },
            { segment: 'GenerateRegularReports', title: 'Generate Regular Reports', icon: <DocumentScannerOutlined /> , permission: ['Permissions.Reports.Generate']},
        ],
    },
];
const demoTheme = createTheme({
    cssVariables: { colorSchemeSelector: 'data-toolpad-color-scheme' },
    colorSchemes: { light: true, dark: true },
    breakpoints: { values: { xs: 0, sm: 600, md: 960, lg: 1200, xl: 1536 } },
    typography: { fontSize: 11 },
    components: {
        MuiDrawer: {
            styleOverrides: { paper: { width: 100 } },
        },
    },
});


const MainLayout: React.FC<{ children: React.ReactNode }> = ({ children }) => {
    const [session, setSession] = useState<Session | null>();
    const [signOutModalOpen, setSignOutModalOpen] = useState(false);
    const [sessionExpiredModalOpen, setSessionExpiredModalOpen] = useState(false);
    // const navigate = useNavigate();

    const now = moment();

    const handleSignOutModalConfirm = () => {
        localStorage.clear();
        toast.dismiss();
        setSession(null);
        authService.logout()
        setTimeout(() => {
            window.location.href='/';
        }, 100);
       
       
    };

    useEffect(() => {

        // console.log(session);
        authentication.signIn()


        const token = localStorage.getItem('token');


        if (token) {
            try {
                const decoded = jwtDecode<CustomJwtPayload>(token);
                const expTime = moment.unix(decoded.exp);

                if (now.isAfter(expTime)) {
                    console.log('Token has expired!');
                    setSessionExpiredModalOpen(true);
                }

            } catch (error) {
                console.error('Invalid token:', error);
            }
        } else {
            handleSignOutModalConfirm();
        }
    }, []);

    const authentication = React.useMemo(
        () => ({
            signIn: () => {
                if (localStorage) {

                    setSession({
                        user: {
                            name: decrypt(Cookies.get('LoginName')),
                            email: decrypt(Cookies.get('Role')),
                        }
                    })

                }

            },
            signOut: () => {
                setSignOutModalOpen(true);

            },
        }),
        []
    );

    return (
        <>

            <AppProvider
                session={session}
                authentication={authentication}
                navigation={filteredNavigation(NAVIGATION, userPermissions)}
                theme={demoTheme}
                branding={{ title: 'CCEMS', logo: '' }}
            >
                <DashboardLayout>
                    <Box
                        sx={{
                            display: 'flex',
                            flexDirection: 'column',
                            flexGrow: 1,
                            height: '100%',
                            padding: 2,
                            boxSizing: 'border-box',
                        }}
                    >
                        <Card
                            sx={{
                                flexGrow: 1,
                                display: 'flex',
                                flexDirection: 'column',
                                justifyContent: 'flex-start',
                                alignItems: 'stretch',
                                padding: 3,
                                boxShadow: 3,
                                overflowY: 'auto'
                            }}
                        >
                            {children}
                        </Card>
                    </Box>
                </DashboardLayout>
            </AppProvider>


            <ConfirmationModal
                open={signOutModalOpen}
                title="Sign out"
                content="Are you sure you want to logout?"
                handleClose={() => setSignOutModalOpen(false)}
                handleConfirm={handleSignOutModalConfirm}
                buttonName="Sign out"
                id={null}
            />


            <ConfirmationModal
                open={sessionExpiredModalOpen}
                title="Session Expired"
                content="Your session has timed out. Please log in again."
                handleClose={() => setSignOutModalOpen(false)}
                handleConfirm={handleSignOutModalConfirm}
                buttonName="Sign In"
                id={null}
            />
        </>
    );
};

export default MainLayout;
