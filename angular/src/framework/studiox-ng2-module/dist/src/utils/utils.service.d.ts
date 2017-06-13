export declare class UtilsService {
    getCookieValue(key: string): string;
    setCookieValue(key: string, value: string, expireDate?: Date, path?: string): void;
    deleteCookie(key: string, path?: string): void;
}
