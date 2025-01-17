/* eslint-disable @typescript-eslint/no-unused-vars */
import React, { useState } from "react";
import {
  Box,
  TextField,
  Button,
  Typography,
  Container,
  Alert,
} from "@mui/material";
import { LoginFormStyle } from "./LoginStyle";
import authService from '../../services/authService';
import { ERROR_MESSAGES, CustomJwtPayload } from '../../utils/constants';
import { jwtDecode } from 'jwt-decode';;
import Cookies from 'js-cookie';
import { encrypt } from "../../utils/encrypt-decrypt";
import ToastService from "../../utils/toast";


// import {Cookies} from 'js-cookie';


const Login: React.FC = () => {
  // const navigate = useNavigate();
  // const EMPTY_FIELDS_ERROR_MSG = 'Username and password is required.';
  const [errorMsg, setErrorMsg] = useState<string | null>(null);
  const [erroInput, setErroInput] = useState({
    password: false,
    username: false,
  });
  const [isLoading, setIsLoading] = useState(false);


  const handleSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    // console.log(errorMsg);
    const data = new FormData(event.currentTarget);
    const username = data.get("username") as string;
    const password = data.get("password") as string;

    setErroInput({
      username: !username,
      password: !password,
    });

    setErrorMsg(null);



    if (username && password) {


      try {
        setIsLoading(true);

        // Reset error before attempting login
        const response = await authService.login({ username, password });

        if (response.isAuthenticated) {
          localStorage.setItem('token', response.token);

          const decoded = jwtDecode<CustomJwtPayload>(response.token);
          console.log(decoded);
          Cookies.set('LoginName', encrypt(decoded.LoginName), { expires: 7 }); // Expires in 7 days
          Cookies.set('Role', encrypt(decoded.Role), { expires: 7 });
          Cookies.set('Permission', decoded.Permission, { expires: 7 });
          Cookies.set('RoleID', encrypt(decoded.RoleID), { expires: 7 });

          // localStorage.setItem('LoginName', decoded.LoginName)
          // localStorage.setItem('Role', decoded.Role)

          window.location.href='/Homepage';
          ToastService.success('Login Successful.')
        } else {
          setErrorMsg(response.message);
        }

      } catch (err) {
        setErrorMsg("An error occurred during login.");
      } finally {
        setIsLoading(false);
      }
    }


  };

  return (
    <Container component="main" maxWidth="xs" sx={LoginFormStyle.container}>
      <Box sx={LoginFormStyle.boxContainer}>
        <Typography component="h1" variant="h5">
          CCEMS
        </Typography>

        {errorMsg && (
          <Alert severity="error" sx={LoginFormStyle.errorMessage}>
            {errorMsg}
          </Alert>
        )}

        <Box
          component="form"
          onSubmit={handleSubmit}
          noValidate
          sx={LoginFormStyle.container2}
        >
          <TextField
            error={erroInput.username}
            margin="normal"
            required
            fullWidth
            id="username"
            label="Username"
            name="username"
            autoComplete="username"
            autoFocus
            helperText={
              erroInput.username ? ERROR_MESSAGES.REQUIRED_FIELD : ""
            }
          />
          <TextField
            error={erroInput.password}
            margin="normal"
            required
            fullWidth
            name="password"
            label="Password"
            type="password"
            id="password"
            autoComplete="current-password"
            helperText={
              erroInput.password ? ERROR_MESSAGES.REQUIRED_FIELD : ""
            }
          />
          <Button
            type="submit"
            fullWidth
            variant="contained"
            sx={LoginFormStyle.submitButton}
            disabled={isLoading}
          >
            {isLoading ? "Logging in..." : "Login"}
          </Button>
        </Box>
      </Box>
    </Container>
  );
};

export default Login;
