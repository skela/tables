//
//  TableAdapter.m
//  tables
//
//  Created by Aleksander Slater on 07/09/2014.
//  Copyright (c) 2014 Davincium. All rights reserved.
//

#import "TableAdapter.h"
#import "TableSource.h"

#import "TableAdapterCell.h"
#import "TableEditor.h"
#import "TableTextEditor.h"
#import "TableSingleChoiceEditor.h"
#import "TableTimeEditor.h"

#define TITLE_FONT [UIFont systemFontOfSize:16]
#define DETAIL_FONT [UIFont fontWithName:@"Helvetica" size:14]

@interface TableAdapter()
@property(nonatomic,strong) UITableView *tv;
@property(nonatomic,strong) TableSource *td;
@end

@implementation TableAdapter

- (id)initWithTable:(UITableView*)table andData:(NSObject *)data usingConfigs:(id<ITableAdapterRowConfigurator>)configs
{
    self = [super init];
    if (self)
    {
        self.shouldAdjustTextContentInset=YES;
        self.detailTextColor = [UIColor darkGrayColor];
        
        self.td = [[TableSource alloc] initWithData:data];
        self.rowConfigurator = configs;
        self.data = data;
        [self setTableView:table];
    }
    return self;
}

- (id)initWithTable:(UITableView*)table andData:(NSObject *)data
{
    self = [self initWithTable:table andData:data usingConfigs:nil];
    if (self)
    {
        
    }
    return self;
}

- (void)setTableView:(UITableView*)table
{
    self.tv.delegate = nil;
    self.tv.dataSource = nil;
    self.tv = nil;
    self.tv = table;
    self.tv.delegate = self;
    self.tv.dataSource = self;
}

- (void)setData:(NSObject *)data
{
    self.td = [[TableSource alloc] initWithData:data];
    [self reloadData];
}

- (NSObject*)data
{
    return self.td.data;
}

- (void)reloadData
{
    [self.tv reloadData];
}

#pragma mark - UITableView

- (NSInteger)numberOfSectionsInTableView:(UITableView *)tableView
{
    return self.td.numberOfSections;
}

- (NSInteger)tableView:(UITableView *)tableView numberOfRowsInSection:(NSInteger)section
{
    return [self.td rowsInSection:section];
}

- (CGFloat)getHeightForRow:(UITableView*)tableView atIndexPath:(NSIndexPath*)indexPath
{
    TableRowType rowType = [self.td rowType:self.rowConfigurator row:indexPath.row section:indexPath.section];
    if (rowType == TableRowTypeBlurb)
    {
        NSString* name = [self.td displayName:self.rowConfigurator row:indexPath.row section:indexPath.section];
        NSString* text = [self.td getValueStringAtRow:indexPath.row andSection:indexPath.section];
        CGFloat contentMargin = 10;
        CGFloat contentWidth = tableView.bounds.size.width;
        CGSize constraint = CGSizeMake(contentWidth - (contentMargin * 2), 20000.0f);
        
        CGSize size = [TextHelper stringSize:text font:DETAIL_FONT constraint:constraint lineBreakMode:NSLineBreakByWordWrapping];
        CGSize sizeT = [TextHelper stringSize:name font:TITLE_FONT];
        CGFloat height = MAX(size.height,44.0f) + sizeT.height;
        return height + (contentMargin * 4);
    }
    return 44;
}

- (CGFloat)tableView:(UITableView *)tableView heightForRowAtIndexPath:(NSIndexPath *)indexPath
{
    return [self getHeightForRow:tableView atIndexPath:indexPath];
}

- (CGFloat)tableView:(UITableView *)tableView estimatedHeightForRowAtIndexPath:(NSIndexPath *)indexPath
{
    return [self getHeightForRow:tableView atIndexPath:indexPath];
}

- (UITableViewCell*)tableView:(UITableView *)tableView cellForRowAtIndexPath:(NSIndexPath *)indexPath
{
    NSString *name = [self.td rowName:indexPath.row andSection:indexPath.section];
    BOOL editable = [self.td rowEditable:self.rowConfigurator row:indexPath.row section:indexPath.section];
    TableRowType rowType = [self.td rowType:self.rowConfigurator row:indexPath.row section:indexPath.section];
    NSObject *value = [self.td getValueAtRow:indexPath.row andSection:indexPath.section];
    TableAdapterCell *cell = (TableAdapterCell*)[tableView dequeueReusableCellWithIdentifier:name];
    if (cell == nil )
    {
        cell = [[TableAdapterCell alloc] initWithStyle:(rowType==TableRowTypeBlurb?UITableViewCellStyleSubtitle:UITableViewCellStyleValue1) reuseIdentifier:name];
        cell.accessoryType = UITableViewCellAccessoryNone;
        //cell = new TableAdapterCell(UITableViewCellStyle.Subtitle,name);
        switch (rowType)
        {
            case TableRowTypeCheckbox:
            {
                TableAdapterRowConfig*c=[self.td rowSetting:self.rowConfigurator propertyName:name];
                if (c == nil || !c.simpleCheckbox)
                {
                    UISwitch* sw = [[UISwitch alloc] init];
                    sw.userInteractionEnabled = false;
                    cell.accessoryView = sw;
                }
            } break;
            case TableRowTypeText:
            {
                TableAdapterRowConfig*c=[self.td rowSetting:self.rowConfigurator propertyName:name];
                if (c != nil && c.inlineTextEditing)
                {
                    UITextField* tf = [[UITextField alloc]initWithFrame:CGRectMake(0, 0, 160, 44)];
                    tf.userInteractionEnabled = true;
                    tf.borderStyle = UITextBorderStyleNone;
                    tf.autoresizingMask = UIViewAutoresizingFlexibleWidth;
                    tf.font = DETAIL_FONT;
                    tf.textColor = self.detailTextColor;
                    tf.textAlignment = NSTextAlignmentRight;
                    tf.delegate = self;
                    [tf addTarget:self action:@selector(textFieldChanged:) forControlEvents:UIControlEventEditingChanged];
                    TableAdapterInlineTextInputAccessoryView* inp = [[TableAdapterInlineTextInputAccessoryView alloc] initWithConfig:c andWidth:tableView.frame.size.width];
                    [inp.previousButton addTarget:self action:@selector(clickedPrevious:) forControlEvents:UIControlEventTouchUpInside];
                    [inp.nextButton addTarget:self action:@selector(clickedNext:) forControlEvents:UIControlEventTouchUpInside];
                    [inp.dismissButton addTarget:self action:@selector(clickedDismiss:) forControlEvents:UIControlEventTouchUpInside];
                    tf.inputAccessoryView = inp;
                    [TableEditor configureTextControl:c control:tf];
                    cell.accessoryView = tf;
                }
                else
                {
                    cell.detailTextLabel.textColor = self.detailTextColor;
                }
            } break;
            case TableRowTypeBlurb:
            {
                cell.textLabel.font = TITLE_FONT;
                cell.detailTextLabel.numberOfLines = 0;
                cell.detailTextLabel.font = DETAIL_FONT;
                cell.detailTextLabel.clipsToBounds = true;
                cell.detailTextLabel.textColor = self.detailTextColor;
                cell.clipsToBounds = true;
            } break;
            default:
            {
                cell.detailTextLabel.textColor = self.detailTextColor;
            } break;
        }
    }

    cell.textLabel.text = [self.td displayName:self.rowConfigurator row:indexPath.row section:indexPath.section];
    cell.selectionStyle = editable ? UITableViewCellSelectionStyleDefault : UITableViewCellSelectionStyleNone;
    switch (rowType)
    {
        case TableRowTypeText:
        {
            NSString* vs = [self.td getValueStringAtRow:indexPath.row andSection:indexPath.section];
            TableAdapterRowConfig*c=[self.td rowSetting:self.rowConfigurator propertyName:name];
            if (c != nil && c.inlineTextEditing)
            {
                UITextField *tf = (UITextField*)cell.accessoryView;
                tf.enabled = editable;
                tf.text = vs;
                tf.secureTextEntry = c.secureTextEditing;
                TableAdapterInlineTextInputAccessoryView*inp = (TableAdapterInlineTextInputAccessoryView*)tf.inputAccessoryView;
                inp.indexPath = indexPath;
                cell.detailTextLabel.text = @"";
            }
            else
            {
                cell.detailTextLabel.text = c != nil && c.secureTextEditing ? [TextHelper scrambledText:vs] : vs;
                cell.accessoryType = editable ? UITableViewCellAccessoryDisclosureIndicator : UITableViewCellAccessoryNone;
            }
        } break;

        case TableRowTypeBlurb:
        {
            NSString* vs = [self.td getValueStringAtRow:indexPath.row andSection:indexPath.section];
            TableAdapterRowConfig*c=[self.td rowSetting:self.rowConfigurator propertyName:name];
            cell.detailTextLabel.text = c != nil && c.secureTextEditing ? [TextHelper scrambledText:vs] : vs;
            cell.accessoryType = editable ? UITableViewCellAccessoryDisclosureIndicator : UITableViewCellAccessoryNone;
        } break;
            
        case TableRowTypeSingleChoiceList:
        {
            NSString* choiceString = nil;
            if (value != nil)
                choiceString = [value description];
            cell.detailTextLabel.text = choiceString;
            cell.accessoryType = editable ? UITableViewCellAccessoryDisclosureIndicator : UITableViewCellAccessoryNone;
        } break;
        
        case TableRowTypeCheckbox:
        {
            cell.detailTextLabel.text = @"";
            BOOL isOn = [self.td getValueBoolAtRow:indexPath.row andSection:indexPath.section];
            TableAdapterRowConfig*s=[self.td rowSetting:self.rowConfigurator propertyName:name];
            if (s != nil && s.simpleCheckbox)
                cell.accessoryType = isOn ? UITableViewCellAccessoryCheckmark : UITableViewCellAccessoryNone;
            else
            {
                UISwitch *sw = (UISwitch*)cell.accessoryView;
                sw.on = isOn;
            }
        } break;

        case TableRowTypeDateTime:
        case TableRowTypeDate:
        case TableRowTypeTime:
        {
            NSDate *date = [self.td getValueDateAtRow:indexPath.row andSection:indexPath.section];
            [self.td displayDate:self.rowConfigurator row:indexPath.row section:indexPath.section date:date rowType:rowType];
            cell.accessoryType = editable ? UITableViewCellAccessoryDisclosureIndicator : UITableViewCellAccessoryNone;
        } break;
            
        case TableRowTypeUnknown:
        {
            
        } break;
    }
    return cell;
}

- (void)tableView:(UITableView *)tableView didSelectRowAtIndexPath:(NSIndexPath *)indexPath
{
    NSString *name = [self.td rowName:indexPath.row andSection:indexPath.section];
    BOOL editable = [self.td rowEditable:self.rowConfigurator row:indexPath.row section:indexPath.section];
    if (!editable)
        return;
    TableRowType rowType = [self.td rowType:self.rowConfigurator row:indexPath.row section:indexPath.section];
    if (self.rowSelector != nil)
        if ([self.rowSelector  tableAdapter:self didSelectRow:name])
                return;
    TableAdapterRowConfig*config=[self.td rowSetting:self.rowConfigurator propertyName:name];
    if (config != nil)
    {
        if (config.clicked!=nil)
        {
            [self performSelectorOnMainThread:config.clicked withObject:nil waitUntilDone:YES];
            return;
        }
    }
    switch (rowType)
    {
        case TableRowTypeCheckbox:
        {
            NSNumber* number = [self.td getValueNumberAtRow:indexPath.row andSection:indexPath.section];
            BOOL isOn = ![number boolValue];
            NSNumber *newValue = @(isOn);
            [self.td setValue:newValue atRow:indexPath.row andSection:indexPath.section];
            [tableView reloadRowsAtIndexPaths:@[indexPath] withRowAnimation:UITableViewRowAnimationFade];
            [self changedValue:name oldValue:number newValue:newValue];
        } break;
        case TableRowTypeText:
        case TableRowTypeBlurb:
        {
            UIViewController* tvc = self.firstAvailableViewController;
            if (tvc != nil)
            {
                NSString* str = [self.td getValueStringAtRow:indexPath.row andSection:indexPath.section];
                if (config != nil && config.inlineTextEditing && rowType==TableRowTypeText)
                {
                    UITableViewCell *cell = [tableView cellForRowAtIndexPath:indexPath];
                    UITextField* tf = (UITextField*)cell.accessoryView;
                    [tf becomeFirstResponder];
                }
                else
                {
                    NSString* dname = [self.td displayName:self.rowConfigurator row:indexPath.row section:indexPath.section];
                    TableTextEditor* textEditor = [[TableTextEditor alloc] initWithRowType:rowType title:dname value:str changed:^(NSString*changedString)
                    {
                        [self.td setValue:changedString atRow:indexPath.row andSection:indexPath.section];
                        [self reloadData];
                    }];
                    [textEditor setShouldAdjustTextContentInset:self.shouldAdjustTextContentInset];
                    [textEditor configure:config];
                    [TableAdapter presentScreen:textEditor fromParent:tvc];
                }
            }
        } break;
        case TableRowTypeSingleChoiceList:
        {
            UIViewController *sclvc = self.firstAvailableViewController;
            if (sclvc != nil)
            {
                NSString* dname = [self.td displayName:self.rowConfigurator row:indexPath.row section:indexPath.section];
                NSObject* value = [self.td getValueAtRow:indexPath.row andSection:indexPath.section];
                TableSingleChoiceEditor* singleChoiceEditor = [[TableSingleChoiceEditor alloc] initWithRowType:rowType
                                                                                                         title:dname
                                                                                                  chosenOption:value
                                                                                                        config:config
                                                                                                       changed:^(NSObject* changedChoice)
                {
                    [self.td setValue:changedChoice atRow:indexPath.row andSection:indexPath.section];
                    [self reloadData];
                }];
                [TableAdapter presentScreen:singleChoiceEditor fromParent:sclvc];
            }
        }  break;
        case TableRowTypeDate:
        case TableRowTypeTime:
        case TableRowTypeDateTime:
        {
            UIViewController *dvc = self.firstAvailableViewController;
            if (dvc != nil)
            {
                NSString* dname = [self.td displayName:self.rowConfigurator row:indexPath.row section:indexPath.section];
                NSDate* value = [self.td getValueDateAtRow:indexPath.row andSection:indexPath.section];
                UIDatePickerMode mode = UIDatePickerModeDateAndTime;
                if (rowType == TableRowTypeDate)
                    mode = UIDatePickerModeDate;
                else if (rowType == TableRowTypeTime)
                    mode = UIDatePickerModeTime;
                TableTimeEditor* dateEditor = [[TableTimeEditor alloc] initWithTitle:dname value:value mode:mode changed:^(NSDate*changedDate)
                {
                    [self.td setValue:changedDate atRow:indexPath.row andSection:indexPath.section];
                    [self reloadData];
                }];
                [TableAdapter presentScreen:dateEditor fromParent:dvc];
            }
        } break;
            
        case TableRowTypeUnknown:
        {
            
        } break;
    }
}

- (void)changedValue:(NSString*)rowName oldValue:(NSObject*)oldValue newValue:(NSObject*)newValue
{
    [self.rowChanged rowChanged:self rowName:rowName oldValue:oldValue newValue:newValue];
}

+ (void)presentScreen:(UIViewController*)screen fromParent:(UIViewController*)parent
{
    if (parent.navigationController == nil)
        [parent presentViewController:[[UINavigationController alloc]initWithRootViewController:screen] animated:YES completion:nil];
    else
        [parent.navigationController pushViewController:screen animated:YES];
}

- (UIViewController*)firstAvailableViewController
{
    return (UIViewController*)[TableAdapter traverseResponderChainForViewController:self.tv];
}

+ (UIResponder*)traverseResponderChainForViewController:(UIView*)v
{
    UIResponder *responder = [v nextResponder];
    if ([responder isKindOfClass:[UIViewController class]])
    {
        return responder;
    }
    else if ([responder isKindOfClass:[UIView class]])
    {
        return [TableAdapter traverseResponderChainForViewController:(UIView*)responder];
    }
    else
    {
        return nil;
    }
}

#pragma mark - Inline Text Editing

- (NSIndexPath*)nextIndexPath:(NSIndexPath*)indexPath
{
    NSInteger numOfSections = [self numberOfSectionsInTableView:self.tv];
    NSInteger nextSection = ((indexPath.section + 1) % numOfSections);
    
    if ((indexPath.row + 1) == [self tableView:self.tv numberOfRowsInSection:indexPath.section])
    {
        return [NSIndexPath indexPathForRow:0 inSection:nextSection];
    }
    else
    {
        return [NSIndexPath indexPathForRow:indexPath.row + 1 inSection:indexPath.section];
    }
}

- (NSIndexPath*)previousIndexPath:(NSIndexPath*)indexPath
{
    NSInteger numOfSections = [self numberOfSectionsInTableView:self.tv];
    NSInteger nextSection = ((indexPath.section - 1) % numOfSections);
    if ((indexPath.row - 1) < 0)
    {
        return [NSIndexPath indexPathForRow:0 inSection:nextSection];
    }
    else
    {
        return [NSIndexPath indexPathForRow:indexPath.row - 1 inSection:indexPath.section];
    }
}

- (IBAction)clickedPrevious:(id)sender
{
    UIView *v = (UIView *)sender;
    TableAdapterInlineTextInputAccessoryView *ac = (TableAdapterInlineTextInputAccessoryView*)[v superview];
    NSIndexPath *indexPath =ac.indexPath;
    NSIndexPath *nextIndexPath = [self previousIndexPath:indexPath];
    [self.tv selectRowAtIndexPath:nextIndexPath animated:YES scrollPosition:UITableViewScrollPositionTop];
    [self tableView:self.tv didSelectRowAtIndexPath:nextIndexPath];
}

- (IBAction)clickedNext:(id)sender
{
    UIView *v = (UIView *)sender;
    TableAdapterInlineTextInputAccessoryView *ac = (TableAdapterInlineTextInputAccessoryView*)[v superview];
    NSIndexPath *indexPath =ac.indexPath;
    NSIndexPath *nextIndexPath = [self nextIndexPath:indexPath];
    [self.tv selectRowAtIndexPath:nextIndexPath animated:YES scrollPosition:UITableViewScrollPositionTop];
    [self tableView:self.tv didSelectRowAtIndexPath:nextIndexPath];
}

- (IBAction)clickedDismiss:(id)sender
{
    [[[UIApplication sharedApplication] keyWindow] endEditing:YES];
}

- (void)textFieldChanged:(UITextField*)tf
{
    TableAdapterInlineTextInputAccessoryView*inp = (TableAdapterInlineTextInputAccessoryView*)tf.inputAccessoryView;
    NSIndexPath *indexPath = inp.indexPath;
    [self.td setValue:tf.text atRow:indexPath.row andSection:indexPath.section];
}

-(BOOL)textFieldShouldReturn:(UITextField *)tf
{
    TableAdapterInlineTextInputAccessoryView*inp = (TableAdapterInlineTextInputAccessoryView*)tf.inputAccessoryView;
    NSIndexPath *indexPath = inp.indexPath;
    NSIndexPath *nextIndexPath = [self nextIndexPath:indexPath];
    [self.tv selectRowAtIndexPath:nextIndexPath animated:YES scrollPosition:UITableViewScrollPositionTop];
    [self tableView:self.tv didSelectRowAtIndexPath:nextIndexPath];
    return YES;
}

//- (BOOL)textField:(UITextField *)tf shouldChangeCharactersInRange:(NSRange)range replacementString:(NSString *)replacement
//{
//    NSString* text = tf.text;
//    tf.Text = text.Substring (0, range.Location) + replacement + text.Substring (range.Location + range.Length);
//    TextFieldChanged (tf);
//    return false;
//}

@end
