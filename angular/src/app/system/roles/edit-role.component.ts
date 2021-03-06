import { Component, ViewChild, Injector, Output, EventEmitter, ElementRef, OnInit } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { RoleServiceProxy, PermissionServiceProxy, RoleDto, ListResultDtoOfPermissionDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/app-component-base';

@Component({
    selector: 'edit-role-modal',
    templateUrl: './edit-role.component.html'
})
export class EditRoleComponent extends AppComponentBase implements OnInit {
@ViewChild('editRoleModal') modal: ModalDirective;
@ViewChild('modalContent') modalContent: ElementRef;
    
    active: boolean = false;
    saving: boolean = false;

    permissions: ListResultDtoOfPermissionDto = null;
    role: RoleDto = null;

    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();
    constructor(
        injector: Injector,
        private _permissionService: PermissionServiceProxy,
        private _roleService: RoleServiceProxy
    ) {
        super(injector);
    }

	ngOnInit(): void {
        this._permissionService.getAll()
            .subscribe((permissions:ListResultDtoOfPermissionDto) => 
            {
                this.permissions = permissions;
            });
	}

	show(id:number): void {
		this._roleService.get(id)
		.finally(() => {
			this.active = true;
        	this.modal.show();
		})
		.subscribe((result)=>{
			this.role = result;
		});
	}

	onShown(): void {
        ($ as any).AdminBSB.input.activate($(this.modalContent.nativeElement));

        $('#form_edit_role').validate({
            highlight: function (input) {
                $(input).parents('.form-line').addClass('error');
            },
            unhighlight: function (input) {
                $(input).parents('.form-line').removeClass('error');
            },
            errorPlacement: function (error, element) {
                $(element).parents('.form-group').append(error);
            }
        });
    }

	checkPermission(permissionName: string): string {
		if(this.role.permissions.indexOf(permissionName) != -1)
		{
			return "checked";
		}
		else
		{
			return "";
		}
	}

    save(): void {
        var permissions = [];
        $(this.modalContent.nativeElement).find("[name=permission]").each(
            function (index:number, elem: Element){
                if($(elem).is(":checked") == true){
                    permissions.push(elem.getAttribute("value").valueOf());
                }
            }
        )

        this.role.permissions = permissions;
        this.saving = true;
        this._roleService.update(this.role)
            .finally(() => { this.saving = false; })
            .subscribe(() => {
                this.notify.info(this.l('SavedSuccessfully'));
                this.close();
                this.modalSave.emit(null);
            });
    }

	close(): void {
        this.active = false;
        this.modal.hide();
    }
}
