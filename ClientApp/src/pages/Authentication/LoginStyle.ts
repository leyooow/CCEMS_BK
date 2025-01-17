export const LoginFormStyle = {
    container: {
        display: 'flex',
        flexDirection: 'column',
        alignItems: 'center',
        justifyContent: 'center', // Centers content vertically
        height: '100vh', // Full viewport height
        
    },
    boxContainer: {
        textAlign:'center',
        width: '100%', // Full viewport width
        maxWidth: '400px', // Optional: restrict the container width
        margin: '0 auto', // Centers the container horizontally
        boxShadow: '0 4px 10px rgba(0, 0, 0, 0.1)', // Adds shadow
        borderRadius: '12px', // Rounded corners
        padding: '30px', // Padding inside the container
        boxSizing: 'border-box', // Ensures padding is included in the element's total width/height
    },
    submitButton: {
        marginTop: 3,
        marginButtom: 2,
    },
    container2: {
        marginTop: 1,
    },
    errorMessage: {
        marginTop: 2,
    },
};