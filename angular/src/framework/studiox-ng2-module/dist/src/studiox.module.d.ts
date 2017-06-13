import { Http, XHRBackend, RequestOptions } from '@angular/http';
import { StudioXHttpConfiguration } from './studioxHttp';
export declare function studioxHttpFactory(xhrBackend: XHRBackend, requestOptions: RequestOptions, configuration: StudioXHttpConfiguration): Http;
export declare let StudioXHttpProvider: {
    provide: any;
    useFactory: (xhrBackend: any, requestOptions: any, configuration: StudioXHttpConfiguration) => any;
    deps: any[];
};
export declare class StudioXModule {
}
