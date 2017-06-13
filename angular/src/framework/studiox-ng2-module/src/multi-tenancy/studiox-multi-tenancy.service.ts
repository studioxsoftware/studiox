import { Injectable } from '@angular/core';

@Injectable()
export class StudioXMultiTenancyService {

    get isEnabled(): boolean {
        return studiox.multiTenancy.isEnabled;
    }

}