import CryptoJS from 'crypto-js';
import { SECRET_KEY } from './constants';



export const encrypt = (value: string) => {
    return CryptoJS.AES.encrypt(value, SECRET_KEY).toString();
}

// Decrypt a value
export const decrypt = (value: any) => {
    const bytes = CryptoJS.AES.decrypt(value, SECRET_KEY);
    return bytes.toString(CryptoJS.enc.Utf8);
}