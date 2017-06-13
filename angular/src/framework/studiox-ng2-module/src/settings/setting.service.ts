import { Injectable } from '@angular/core';

@Injectable()
export class SettingService {

    get(name: string): string {
        return studiox.setting.get(name);
    }

    getBoolean(name: string): boolean {
        return studiox.setting.getBoolean(name);
    }
    
    getInt(name: string): number {
        return studiox.setting.getInt(name);
    }

}