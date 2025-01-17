export interface FormData {
    [key: string]: FormField;
}

interface FormField {
    value: string | string[];
    error: boolean;
    helperText: string;
}

