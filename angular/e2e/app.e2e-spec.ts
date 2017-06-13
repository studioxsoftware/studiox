import { StudioXBoilerplateTemplatePage } from './app.po';

describe('StudioX Boilerplate App', function() {
  let page: StudioXBoilerplateTemplatePage;

  beforeEach(() => {
    page = new StudioXBoilerplateTemplatePage();
  });

  it('should display message saying app works', () => {
    page.navigateTo();
    expect(page.getParagraphText()).toEqual('app works!');
  });
});
