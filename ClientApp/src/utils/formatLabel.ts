// Utility to convert camelCase or snake_case to a more readable label with spaces
export const formatLabel = (key: string): string => {
    return key
        .replace(/([a-z])([A-Z])/g, '$1 $2') // Insert space between camelCase words
        .replace(/_/g, ' ') // Replace underscores with spaces
        .replace(/\b\w/g, (char) => char.toUpperCase()); // Capitalize each word
};
