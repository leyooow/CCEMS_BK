import { createTheme } from '@mui/material/styles';

export const globalStyle = {
  mainBox: {
    marginBottom: 0,
    display: 'flex',
    justifyContent: 'space-between',
  },
  searchBox: {
    display: 'flex',
    gap: .5,
    alignItems: 'center',
    marginBottom: 2
  },
  searchInput: {
    flex: 1,
  },
  buttonBox: {
    display: 'flex'
  },
  buttonRed: {
    color: 'red'
  }
}


export const buttomTheme = createTheme({
  palette: {
    primary: {
      main: '#1976d2',
    },
    secondary: {
      main: '#dc004e',
    },
  },
  components: {
    MuiButton: {
      styleOverrides: {
        root: {
          borderRadius: 8, // Example: Custom rounded corners
        },
      },
    },
  },
});
