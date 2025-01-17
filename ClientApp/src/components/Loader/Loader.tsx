import React from "react";
import CircularProgress from "@mui/material/CircularProgress";
import Box from "@mui/material/Box";
import Typography from "@mui/material/Typography";

interface LoaderProps {
  size?: number;
  color?: "primary" | "secondary" | "inherit";
  fullScreen?: boolean;
  message?: string; // Optional loading message
}

const Loader: React.FC<LoaderProps> = ({
  size = 80,
  color = "primary",
  fullScreen = true,
  message = "loading...",
}) => {
  return (
    <Box
      sx={{
        position: fullScreen ? "fixed" : "relative",
        top: 0,
        left: 0,
        width: "100%",
        height: fullScreen ? "100vh" : "auto",
        backgroundColor: "transparent", // Transparent background
        display: "flex",
        flexDirection: "column", // Stack the loader and message vertically
        justifyContent: "center",
        alignItems: "center",
        zIndex: 1300, // Ensure it's above other elements
        textAlign: "center",
        padding: 2,
      }}
    >
      <CircularProgress size={size} color={color} />
      {message && (
        <Typography
          variant="h6"
          sx={{
            marginTop: 2,
            fontWeight: 500,
            color: "#333", // You can customize the message text color
          }}
        >
          {message}
        </Typography>
      )}
    </Box>
  );
};

export default Loader;
