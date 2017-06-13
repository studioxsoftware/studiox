import { Http, XHRBackend, Request, Response, RequestOptionsArgs, RequestOptions } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import { MessageService } from './message/message.service';
import { LogService } from './log/log.service';
import 'rxjs/add/operator/catch';
import 'rxjs/add/operator/map';
export interface IValidationErrorInfo {
    message: string;
    members: string[];
}
export interface IErrorInfo {
    code: number;
    message: string;
    details: string;
    validationErrors: IValidationErrorInfo[];
}
export interface IAjaxResponse {
    success: boolean;
    result?: any;
    targetUrl?: string;
    error?: IErrorInfo;
    unAuthorizedRequest: boolean;
    __studiox: boolean;
}
export declare class StudioXHttpConfiguration {
    private _messageService;
    private _logService;
    constructor(_messageService: MessageService, _logService: LogService);
    defaultError: IErrorInfo;
    defaultError401: IErrorInfo;
    defaultError403: IErrorInfo;
    defaultError404: IErrorInfo;
    logError(error: IErrorInfo): void;
    showError(error: IErrorInfo): any;
    handleTargetUrl(targetUrl: string): void;
    handleUnAuthorizedRequest(messagePromise: any, targetUrl?: string): void;
    handleNonStudioXErrorResponse(response: Response): void;
    handleStudioXResponse(response: Response, ajaxResponse: IAjaxResponse): Response;
    getStudioXAjaxResponseOrNull(response: Response): IAjaxResponse;
    handleResponse(response: Response): Response;
    handleError(error: Response): Observable<any>;
}
export declare class StudioXHttp extends Http {
    protected configuration: StudioXHttpConfiguration;
    private _tokenService;
    private _utilsService;
    constructor(backend: XHRBackend, defaultOptions: RequestOptions, configuration: StudioXHttpConfiguration);
    get(url: string, options?: RequestOptionsArgs): Observable<Response>;
    post(url: string, body: any, options?: RequestOptionsArgs): Observable<Response>;
    put(url: string, body: any, options?: RequestOptionsArgs): Observable<Response>;
    delete(url: string, options?: RequestOptionsArgs): Observable<Response>;
    request(url: string | Request, options?: RequestOptionsArgs): Observable<Response>;
    protected normalizeRequestOptions(options: RequestOptionsArgs): void;
    protected addAcceptLanguageHeader(options: RequestOptionsArgs): void;
    protected addTenantIdHeader(options: RequestOptionsArgs): void;
    protected addAuthorizationHeaders(options: RequestOptionsArgs): void;
    private itemExists<T>(items, predicate);
}
