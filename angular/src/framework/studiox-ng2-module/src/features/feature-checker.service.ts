import { Injectable } from '@angular/core';

@Injectable()
export class FeatureCheckerService {

    get(featureName: string): studiox.features.IFeature {
        return studiox.features.get(featureName);
    }

    getValue(featureName: string): string {
        return studiox.features.getValue(featureName);
    }

    isEnabled(featureName: string): boolean {
        return studiox.features.isEnabled(featureName);
    }

}