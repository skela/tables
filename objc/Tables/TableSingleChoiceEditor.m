//
//  TableSingleChoiceEditor.m
//  tables
//
//  Created by Aleksander Slater on 08/09/2014.
//  Copyright (c) 2014 Davincium. All rights reserved.
//

#import "TableSingleChoiceEditor.h"
#import <objc/runtime.h>

@interface TableSingleChoiceEditor ()
@property(nonatomic,readwrite) TableRowType rowType;
@property(nonatomic,strong) NSObject*chosenOption;
@property(nonatomic,strong) NSArray*options;
@property(nonatomic,strong) UIPickerView *picker;
@end

@implementation TableSingleChoiceEditor

- (id)initWithRowType:(TableRowType)type title:(NSString*)aTitle chosenOption:(NSObject*)chosenOption config:(TableAdapterRowConfig*)config changed:(void (^)(NSObject *object))block
{
    self = [super init];
    if (self)
    {
        self.rowType = type;
        self.title = aTitle;
        self.chosenOption = chosenOption;
        self.options = config != nil ? config.singleChoiceOptions : nil;
        
        objc_setAssociatedObject(self, "blockCallback", [block copy], OBJC_ASSOCIATION_RETAIN_NONATOMIC);
    }
    return self;
}

- (void)viewDidLoad
{
    [super viewDidLoad];
    
    self.view.backgroundColor = [UIColor whiteColor];
    
    if (self.navigationItem != nil)
    {
        self.navigationItem.leftBarButtonItem = [[UIBarButtonItem alloc] initWithBarButtonSystemItem:UIBarButtonSystemItemCancel target:self action:@selector(clickedCancel:)];
        self.navigationItem.rightBarButtonItem = [[UIBarButtonItem alloc] initWithBarButtonSystemItem:UIBarButtonSystemItemDone target:self action:@selector(clickedDone:)];
    }
    
    self.picker = [[UIPickerView alloc] initWithFrame:self.view.bounds];
    self.picker.autoresizingMask = UIViewAutoresizingFlexibleTopMargin | UIViewAutoresizingFlexibleWidth;
    self.picker.delegate = self;
    self.picker.dataSource = self;
    [self.view addSubview:self.picker];
}

- (void)viewWillLayoutSubviews
{
    [super viewWillLayoutSubviews];
    self.picker.center = self.view.center;
}

- (void)viewWillAppear:(BOOL)animated
{
    [super viewWillAppear:animated];
    self.picker.center = self.view.center;
}

- (void)viewDidAppear:(BOOL)animated
{
    [super viewDidAppear:animated];
    
    if (self.options!=nil && self.chosenOption!=nil)
    {
        NSInteger index = [self.options indexOfObject:self.chosenOption];
        [self.picker selectRow:index inComponent:0 animated:YES];
    }
}

- (IBAction)clickedCancel:(id)sender
{
    [self closeViewController];
}

- (IBAction)clickedDone:(id)sender
{
    void (^choiceChanged)(NSObject*object) = objc_getAssociatedObject(self, "blockCallback");
    if (choiceChanged != NULL)
    {
        NSInteger index = [self.picker selectedRowInComponent:0];
        NSObject* theChoice = nil;
        if (self.options!=nil)
            theChoice = self.options[index];
        choiceChanged (theChoice);
    }
    [self closeViewController];
}

#pragma mark - Datasource

- (NSInteger)numberOfComponentsInPickerView:(UIPickerView *)pickerView
{
    return 1;
}

- (NSInteger)pickerView:(UIPickerView *)pickerView numberOfRowsInComponent:(NSInteger)component
{
    return self.options == nil ? 0 : self.options.count;
}

#pragma mark - Delegate

- (NSString *)pickerView:(UIPickerView *)pickerView titleForRow:(NSInteger)row forComponent:(NSInteger)component
{
    NSString *returnValue = nil;
    if (self.options != nil)
    {
        NSObject* anObject = self.options [row];
        returnValue = [anObject description];
    }
    return returnValue;
}

@end
