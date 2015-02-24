//
//  TableTimeEditor.m
//  tables
//
//  Created by Aleksander Slater on 08/09/2014.
//  Copyright (c) 2014 Davincium. All rights reserved.
//

#import "TableTimeEditor.h"
#import <objc/runtime.h>

@interface TableTimeEditor ()
@property(nonatomic,strong) NSDate*value;
@property(nonatomic,strong) UIDatePicker*picker;
@property(nonatomic,readwrite) UIDatePickerMode mode;
@end

@implementation TableTimeEditor

- (id)initWithTitle:(NSString*)aTitle value:(NSDate*)aValue mode:(UIDatePickerMode)aMode changed:(void (^)(NSDate *date))block
{
    self = [super init];
    if (self)
    {
        self.title = aTitle;
        self.value = aValue;
        self.mode = aMode;
        
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
    
    self.picker = [[UIDatePicker alloc] initWithFrame:self.view.bounds];
    self.picker.autoresizingMask = UIViewAutoresizingFlexibleTopMargin | UIViewAutoresizingFlexibleWidth;
    self.picker.datePickerMode = self.mode;
    if (self.value!=nil) self.picker.date = self.value;
    [self.view addSubview:self.picker];
}

- (void)viewDidLayoutSubviews
{
    [super viewDidLayoutSubviews];
    self.picker.center = self.view.center;
}

- (IBAction)clickedCancel:(id)sender
{
    [self closeViewController];
}

- (IBAction)clickedDone:(id)sender
{
    void (^dateChanged)(NSDate*date) = objc_getAssociatedObject(self, "blockCallback");
    if (dateChanged != NULL)
        dateChanged(self.picker.date);
    [self closeViewController];
}

@end
