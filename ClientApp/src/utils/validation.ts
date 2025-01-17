// Validate if the input is a valid email address
export const isValidEmail = (email: string): boolean => {
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return emailRegex.test(email);
  };
  
  // Validate if the input is a non-empty string
  export const isNonEmptyString = (value: string): boolean => {
    return value.trim().length > 0;
  };
  
  // Validate if the input is a valid date
  import moment from 'moment';
  export const isValidDate = (date: string, format: string = 'YYYY-MM-DD'): boolean => {
    return moment(date, format, true).isValid();
  };
  
  // Validate if the password meets certain criteria
  export const isValidPassword = (password: string): boolean => {
    // Example: Minimum 8 characters, at least one uppercase, one lowercase, and one number
    const passwordRegex = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$/;
    return passwordRegex.test(password);
  };
  