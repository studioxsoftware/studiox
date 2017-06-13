import { Injectable } from '@angular/core';

@Injectable()
export class LogService {

    debug(logObject?: any): void {
        studiox.log.debug(logObject);
    }

    info(logObject?: any): void {
        studiox.log.info(logObject);
    }

    warn(logObject?: any): void {
        studiox.log.warn(logObject);
    }

    error(logObject?: any): void {
        studiox.log.error(logObject);
    }

    fatal(logObject?: any): void {
        studiox.log.fatal(logObject);
    }

}