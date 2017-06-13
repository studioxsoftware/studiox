import { Injectable } from '@angular/core';

@Injectable()
export class TokenService {

    getToken(): string {
        return studiox.auth.getToken();
    }

    getTokenCookieName(): string {
        return studiox.auth.tokenCookieName;
    }

    clearToken(): void {
        studiox.auth.clearToken();
    }

    setToken(authToken: string, expireDate?: Date): void {
        studiox.auth.setToken(authToken, expireDate);
    }

}