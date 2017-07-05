import { Component, Injector, ViewChild } from '@angular/core';
import { PagedListingComponentBase, PagedRequestDto, EntityDto } from "@shared/paged-listing-component-base";
import { RoleServiceProxy, PermissionServiceProxy, RoleDto } from "shared/service-proxies/service-proxies";

import { appModuleAnimation } from '@shared/animations/routerTransition';
import { CreateRoleComponent } from "@app/system/roles/create-role.component";
import { EditRoleComponent } from "@app/system/roles/edit-role.component";

@Component({
  templateUrl: './roles.component.html',
  animations: [appModuleAnimation()]
})
export class RolesComponent extends PagedListingComponentBase<RoleDto> {

	@ViewChild('createRoleModal') createRoleModal: CreateRoleComponent;
	@ViewChild('editRoleModal') editRoleModal: EditRoleComponent;
	
	roles: RoleDto[] = [];

	constructor(
		private injector:Injector,
		private _permissionService: PermissionServiceProxy,
		private rolesService: RoleServiceProxy
	) {
		super(injector);
	}

	getUIPanelSelector(): string {
		return "div.main-content>>Table";
	}

	toggleActive(role:RoleDto): void {
		studiox.message.confirm(
			"Change role '" + role.displayName + "' to " + (!role.isActive?'active':'inActive'),
			(result:boolean) => {
				if(result) {
					this.rolesService.get(role.id)
						.finally(()=>{})
						.subscribe((result)=>{
							result.isActive = !result.isActive
							
							// update
							this.rolesService.update(result)
									.finally(()=>{
										this.refresh();
									})
									.subscribe((result)=>{});
						});
				}
			}
		);
	}

	list(request: PagedRequestDto, pageNumber: number, finishedCallback: Function): void {
		this.rolesService.getAll(request.skipCount, request.maxResultCount)
			.finally( ()=> {
				finishedCallback();
			})
			.subscribe((result)=>{
				this.roles = result.items;
				this.showPaging(result, pageNumber);
		});
	}

	delete(role: RoleDto): void {
		studiox.message.confirm(
			"Remove Users from Role and delete Role '"+ role.displayName +"'?",
			"Permanently delete this Role",
			(result:boolean) =>{
				if(result)
				{
					this.rolesService.delete(role.id)
						.finally(() => {
							studiox.message.success("Deleted Role: " + role.displayName );
							this.refresh();
						})
						.subscribe(() => {
						});
				}
			}
		);
	}

	// Show Modals
	createRole(): void {
		this.createRoleModal.show();
	}

	editRole(role:RoleDto): void {
		this.editRoleModal.show(role.id);
	}
}
