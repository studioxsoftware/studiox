export declare class TokenService {
    getToken(): string;
    getTokenCookieName(): string;
    clearToken(): void;
    setToken(authToken: string, expireDate?: Date): void;
}
