import {Component, OnInit} from '@angular/core';
import {NotificationTemplateDetailsDto} from "../../../models/notificationTemplateDetailsDto";
import {EditorInstance, EditorOption} from "angular-markdown-editor";
import {FormBuilder, FormGroup, Validators} from "@angular/forms";
import {MarkdownService} from "ngx-markdown";
import {TenantBackendService} from "../../../services/tenant-backend.service";
import {ActivatedRoute} from "@angular/router";
import {MessageService, ObjectCloner} from "@ianitor/shared-services";
import {GraphQLCloneIgnoredProperties} from "../../../../../../ianitor/osp-services/src/lib/shared/graphQL";

@Component({
  selector: 'app-notification-template-details',
  templateUrl: './notification-template-details.component.html',
  styleUrls: ['./notification-template-details.component.css']
})
export class NotificationTemplateDetailsComponent implements OnInit {
  bsEditorInstance: EditorInstance;
  public readonly templateForm: FormGroup;
  editorOptions: EditorOption;
  notificationTemplateDetails: NotificationTemplateDetailsDto;
  private readonly tenantId: string;
  private readonly id: string;
  public loading: boolean;

  constructor(route: ActivatedRoute, private fb: FormBuilder,
              private markdownService: MarkdownService, private dataSourceBackendService: TenantBackendService, private messageService: MessageService) {
    this.tenantId = route.snapshot.paramMap.get('tenantId');
    this.id = route.snapshot.paramMap.get('id');
    this.loading = true;

    this.templateForm = this.fb.group({
      wellKnownName: ['', Validators.required],
      subjectTemplate:[''],
      bodyTemplate: ['', Validators.required],
    });
    this.templateForm.valueChanges.subscribe(formData => {
      if (formData) {
        this.notificationTemplateDetails.bodyTemplate = formData.bodyTemplate;
      }
    });
  }

  ngOnInit() {
    this.notificationTemplateDetails = <NotificationTemplateDetailsDto>{};

    this.editorOptions = {
      autofocus: false,
      iconlibrary: 'fa',
      savable: false,
      onFullscreenExit: (e) => this.hidePreview(e),
      onShow: (e) => this.bsEditorInstance = e,
      parser: (val) => this.parse(val)
    };


    if (this.id) {
      this.dataSourceBackendService.getNotificationTemplateDetails(this.tenantId, this.id).subscribe(value => {
        this.notificationTemplateDetails = value;
        this.templateForm.setValue({
          wellKnownName: value.wellKnownName,
          subjectTemplate: value.subjectTemplate,
          bodyTemplate: value.bodyTemplate,
        });
        this.loading = false;
      })
    } else {
      this.notificationTemplateDetails = <NotificationTemplateDetailsDto>{};
      this.templateForm.setValue({wellKnownName: null, subjectTemplate: null, bodyTemplate: null});
      this.loading = false;
    }
  }


  /** highlight all code found, needs to be wrapped in timer to work properly */
  highlight() {
    setTimeout(() => {
      this.markdownService.highlight();
    });
  }

  hidePreview(e) {
    if (this.bsEditorInstance && this.bsEditorInstance.hidePreview) {
      this.bsEditorInstance.hidePreview();
    }
  }


  parse(inputValue: string) {
    const markedOutput = this.markdownService.compile(inputValue.trim());
    this.highlight();

    return markedOutput;
  }

  save() {

  this.templateForm.disable();
    this.templateForm.updateValueAndValidity();

    const notificationTemplateDetails = ObjectCloner.cloneObject<NotificationTemplateDetailsDto, any>(this.templateForm.value, GraphQLCloneIgnoredProperties);

    if (this.notificationTemplateDetails.rtId) {

      notificationTemplateDetails.rtId = this.notificationTemplateDetails.rtId;

      this.dataSourceBackendService.updateNotificationTemplate(this.tenantId, notificationTemplateDetails).subscribe(value => {

        this.notificationTemplateDetails = value;
        this.messageService.showInformation("Notification template was updated.");
        this.templateForm.enable();
      })

    } else {
      this.dataSourceBackendService.createNotificationTemplate(this.tenantId, notificationTemplateDetails).subscribe(value => {

        this.notificationTemplateDetails = value;
        this.messageService.showInformation("Notification template has been created.");
        this.templateForm.enable();
      })
    }
  }
}
