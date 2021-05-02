import {Component, OnInit} from '@angular/core';
import {ActivatedRoute} from "@angular/router";
import {EditorInstance, EditorOption} from "angular-markdown-editor";
import {MarkdownService} from "ngx-markdown";
import {TenantBackendService} from "../../../services/tenant-backend.service";
import {PageDetailsDto} from "../../../models/pageDetailsDto";
import {FormBuilder, FormGroup, Validators} from "@angular/forms";
import {MessageService, ObjectCloner} from "@ianitor/shared-services";
import {GraphQLCloneIgnoredProperties} from "../../../../../../ianitor/osp-services/src/lib/shared/graphQL";

@Component({
  selector: 'app-page-template-details',
  templateUrl: './page-template-details.component.html',
  styleUrls: ['./page-template-details.component.scss']
})
export class PageTemplateDetailsComponent implements OnInit {

  private readonly tenantId: string;
  private readonly id: string;
  public loading: boolean;
  public readonly editorOptions: EditorOption;
  public bsEditorInstance: EditorInstance;
  public readonly templateForm: FormGroup;

  private pageDetailsDto: PageDetailsDto;

  constructor(route: ActivatedRoute, private fb: FormBuilder, private markdownService: MarkdownService, private dataSourceBackendService: TenantBackendService, private messageService: MessageService) {
    this.tenantId = route.snapshot.paramMap.get('tenantId');
    this.id = route.snapshot.paramMap.get('id');
    this.loading = true;

    this.editorOptions = {
      autofocus: false,
      iconlibrary: 'fa',
      savable: false,
      onFullscreenExit: (e) => this.hidePreview(e),
      onShow: (e) => this.bsEditorInstance = e,
      parser: (val) => this.parse(val)
    };

    this.templateForm = this.fb.group({
      wellKnownName: ['', Validators.required],
      content: []
    });
    this.templateForm.valueChanges.subscribe(formData => {
      if (formData) {
        this.pageDetailsDto.content = formData.content;
      }
    });
  }

  ngOnInit(): void {

    if (this.id) {
      this.dataSourceBackendService.getPageDetails(this.tenantId, this.id).subscribe(value => {
        this.pageDetailsDto = value;

        this.templateForm.setValue({
          wellKnownName: value.wellKnownName,
          content: value.content
        });
        this.loading = false;

      })
    } else {
      this.pageDetailsDto = <PageDetailsDto>{};

      this.templateForm.setValue({wellKnownName: null, content: null});
      this.loading = false;
    }

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

  /** highlight all code found, needs to be wrapped in timer to work properly */
  highlight() {
    setTimeout(() => {
      this.markdownService.highlight();
    });
  }

  showFullScreen(isFullScreen: boolean) {
    if (this.bsEditorInstance && this.bsEditorInstance.setFullscreen) {
      this.bsEditorInstance.showPreview();
      this.bsEditorInstance.setFullscreen(isFullScreen);
    }
  }

  save() {

    this.templateForm.disable();
    this.templateForm.updateValueAndValidity();

    const pageDetailsDto = ObjectCloner.cloneObject<PageDetailsDto, any>(this.templateForm.value, [...GraphQLCloneIgnoredProperties]);


    if (this.pageDetailsDto.rtId) {

      pageDetailsDto.rtId = this.pageDetailsDto.rtId;

      this.dataSourceBackendService.updatePage(this.tenantId, pageDetailsDto).subscribe(updatedEntity => {
        this.pageDetailsDto = updatedEntity;
        this.messageService.showInformation("Page has been updated.");
        this.templateForm.enable();
      })

    } else {
      this.dataSourceBackendService.createPage(this.tenantId, pageDetailsDto).subscribe(updatedEntity => {
        this.pageDetailsDto = updatedEntity;
        this.messageService.showInformation("Page has been created.");
        this.templateForm.enable();

      })
    }
  }
}
