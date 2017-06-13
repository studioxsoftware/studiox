export declare class MessageService {
    info(message: string, title?: string): any;
    success(message: string, title?: string): any;
    warn(message: string, title?: string): any;
    error(message: string, title?: string): any;
    confirm(message: string, callback?: (result: boolean) => void): any;
    confirm(message: string, title?: string, callback?: (result: boolean) => void): any;
}
