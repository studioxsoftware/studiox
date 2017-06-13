import { Injectable } from '@angular/core';

@Injectable()
export class MessageService {

    info(message: string, title?: string): any {
        return studiox.message.info(message, title);
    }

    success(message: string, title?: string): any {
        return studiox.message.success(message, title);
    }

    warn(message: string, title?: string): any {
        return studiox.message.warn(message, title);
    }

    error(message: string, title?: string): any {
        return studiox.message.error(message, title);
    }

    confirm(message: string, callback?: (result: boolean) => void): any;
    confirm(message: string, title?: string, callback?: (result: boolean) => void): any;

    confirm(message: string, titleOrCallBack?: string | ((result: boolean) => void), callback?: (result: boolean) => void): any {
        if (typeof titleOrCallBack == 'string') {
            return studiox.message.confirm(message, titleOrCallBack as string, callback);
        } else {
            return studiox.message.confirm(message, titleOrCallBack as ((result: boolean) => void));
        }
    }

}