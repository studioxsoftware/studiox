import { Injectable } from '@angular/core';

@Injectable()
export class StudioXSessionService {

    get userId(): number | undefined {
        return studiox.session.userId;
    }

    get tenantId(): number | undefined {
        return studiox.session.tenantId;
    }

    get impersonatorUserId(): number | undefined {
        return studiox.session.impersonatorUserId;
    }

    get impersonatorTenantId(): number | undefined {
        return studiox.session.impersonatorTenantId;
    }

    get multiTenancySide(): studiox.multiTenancy.sides {
        return studiox.session.multiTenancySide;
    }

}