import React from 'react';
import { useLocation } from 'react-router-dom';
import MainLayout from './layouts/MainLayout';
import AppRoutes from './routes/AppRoutes';
import { ToastContainer } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';
import 'bootstrap/dist/css/bootstrap.min.css';

const App: React.FC = () => {
  const location = useLocation();

  // Define the routes to exclude from MainLayout
  const excludedRoutes: string[] = ['/login', '/'];
  const isExcludedRoute = excludedRoutes.some(
    (route) => route.toLowerCase() === location.pathname.toLowerCase()
  );

  return (
    <div>
      {/* Other components */}
      <ToastContainer
        position="bottom-right"
        autoClose={2000}
        hideProgressBar={false}
        newestOnTop={false}
        closeOnClick
        rtl={false}
        pauseOnFocusLoss
        draggable
        pauseOnHover
        theme="light"
      />
      {isExcludedRoute ? (
        // Only render AppRoutes for excluded routes (e.g., /login), without MainLayout
        <AppRoutes />
      ) : (
        // Render MainLayout for all other routes
        <MainLayout>
          <AppRoutes />
        </MainLayout>
      )}
    </div>
  );
};

export default App;
