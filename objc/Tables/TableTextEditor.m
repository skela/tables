//
//  TableTextEditor.m
//  tables
//
//  Created by Aleksander Slater on 08/09/2014.
//  Copyright (c) 2014 Davincium. All rights reserved.
//

#import "TableTextEditor.h"
#import <objc/runtime.h>

@interface TableTextEditor ()
@property(nonatomic,strong) NSString *value;
@property(nonatomic,strong) UITextField *textField;
@property(nonatomic,strong) UITextView *textView;
@property(nonatomic,readwrite) TableRowType rowType;
@property(nonatomic,strong) NSNotification *lastNotification;
@end

@implementation TableTextEditor
@synthesize shouldAdjustTextContentInset;

- (id)initWithRowType:(TableRowType)type title:(NSString*)aTitle value:(NSString*)aValue changed:(void (^)(NSString *text))block
{
    self = [super init];
    if (self)
    {
        self.title = aTitle;
        self.value = aValue;
        self.rowType = type;
        self.capitalizationType=UITextAutocapitalizationTypeSentences;
        objc_setAssociatedObject(self, "blockCallback", [block copy], OBJC_ASSOCIATION_RETAIN_NONATOMIC);
    }
    return self;
}

- (void)configure:(TableAdapterRowConfig*)config
{
    if (config!=nil)
    {
        if (config.keyboardType != KeyboardTypeIgnore)
            self.keyboardType = [TableEditor convertKeyboardType:config.keyboardType];
        if (config.capitalizationType != CapitalizationTypeIgnore)
            self.capitalizationType = [TableEditor convertCapitatilizationType:config.capitalizationType];
        if (config.correctionType != CorrectionTypeIgnore)
            self.correctionType = [TableEditor convertCorrectionType:config.correctionType];
        self.secureTextEntry = config.secureTextEditing;
    }
}

///void (^block)(NSString*text) = objc_getAssociatedObject(self, "blockCallback");
//block(self.textView.text);

- (id<UITextInput>)text
{
    if (self.textView != nil)
        return self.textView;
    if (self.textField != nil)
        return self.textField;
    return nil;
}

- (void)viewDidLoad
{
    [super viewDidLoad];
    
    if (self.navigationItem != nil)
    {
        self.navigationItem.leftBarButtonItem = [[UIBarButtonItem alloc] initWithBarButtonSystemItem:UIBarButtonSystemItemCancel target:self action:@selector(clickedCancel:)];
        self.navigationItem.rightBarButtonItem = [[UIBarButtonItem alloc] initWithBarButtonSystemItem:UIBarButtonSystemItemDone target:self action:@selector(clickedDone:)];
    }
    
    if (self.rowType == TableRowTypeBlurb)
    {
        self.textView = [[UITextView alloc] initWithFrame:self.view.bounds];
        self.textView.autoresizingMask = UIViewAutoresizingFlexibleHeight | UIViewAutoresizingFlexibleWidth;
        self.textView.secureTextEntry = self.secureTextEntry;
        self.textView.text = self.value;
        self.textView.autocapitalizationType = self.capitalizationType;
        self.textView.autocorrectionType = self.correctionType;
        self.textView.keyboardType = self.keyboardType;
        self.textView.font = [UIFont systemFontOfSize:14];
        [self.view addSubview:self.textView];
    }
    else if (self.rowType == TableRowTypeText)
    {
        self.view.backgroundColor = [UIColor whiteColor];
        self.textField = [[UITextField alloc] initWithFrame:CGRectMake(10, 10, self.view.bounds.size.width - 20, 44)];
        self.textField.borderStyle = UITextBorderStyleLine;
        self.textField.autoresizingMask = UIViewAutoresizingFlexibleWidth;
        self.textField.text = self.value;
        self.textField.secureTextEntry = self.secureTextEntry;
        self.textField.autocapitalizationType = self.capitalizationType;
        self.textField.autocorrectionType = self.correctionType;
        self.textField.keyboardType = self.keyboardType;
        [self.textField setDelegate:self];
        [self.view addSubview:self.textField];
    }
}

- (void)viewWillAppear:(BOOL)animated
{
    [super viewWillAppear:animated];
    
    if (self.textView != nil && shouldAdjustTextContentInset)
    {
        [self listenToKeyboardNotifications:YES];
    }
    
    if (self.textView != nil)
        [self.textView becomeFirstResponder];
    else if (self.textField != nil)
        [self.textField becomeFirstResponder];
}

- (void)viewDidDisappear:(BOOL)animated
{
    [super viewDidDisappear:animated];
    if (self.textView != nil)
    {
        [self listenToKeyboardNotifications:NO];
    }
}

- (BOOL)textFieldShouldReturn:(UITextField *)textField
{
    [self clickedDone:nil];
    return YES;
}

- (IBAction)clickedCancel:(id)sender
{
    [self closeViewController];
}

- (IBAction)clickedDone:(id)sender
{
    void (^textChanged)(NSString*text) = objc_getAssociatedObject(self, "blockCallback");
    if (textChanged != NULL)
    {
        if (self.textView!=nil)
            textChanged (self.textView.text);
        else if (self.textField!=nil)
            textChanged (self.textField.text);
    }
    [self closeViewController];
}

#pragma mark - Members

- (UITextAutocapitalizationType)capitalizationType
{
    return capitalizationType;
}

- (void)setCapitalizationType:(UITextAutocapitalizationType)value
{
    capitalizationType = value;
    if (self.text != nil)
        self.text.autocapitalizationType = capitalizationType;
}

- (UITextAutocorrectionType)correctionType
{
    return correctionType;
}

- (void)setCorrectionType:(UITextAutocorrectionType)value
{
    correctionType = value;
    if (self.text != nil)
        self.text.autocorrectionType = correctionType;
}

- (BOOL)secureTextEntry
{
    return secureTextEntry;
}

- (void)setSecureTextEntry:(BOOL)value
{
    secureTextEntry = value;
    if (self.text != nil)
        self.text.secureTextEntry = secureTextEntry;
}

- (UIKeyboardType)keyboardType
{
    return keyboardType;
}

- (void)setKeyboardType:(UIKeyboardType)value
{
        keyboardType = value;
        if (self.text != nil)
            self.text.keyboardType = keyboardType;
}

#pragma mark - Keyboard Offset

- (BOOL)extendedLayoutIncludesOpaqueBars
{
    return YES;
}

- (UIRectEdge)edgesForExtendedLayout
{
    return UIRectEdgeNone;
}

- (BOOL)automaticallyAdjustsScrollViewInsets
{
    return NO;
}

- (void)listenToKeyboardNotifications:(BOOL)shouldListen
{
    NSNotificationCenter*c = [NSNotificationCenter defaultCenter];
    if (shouldListen)
    {
        [self listenToKeyboardNotifications:NO];
        [c addObserver:self selector:@selector(handleKeyboardWillHide:) name:UIKeyboardWillHideNotification object:nil];
        [c addObserver:self selector:@selector(handleKeyboardDidHide:) name:UIKeyboardDidHideNotification object:nil];
        [c addObserver:self selector:@selector(handleKeyboardDidShow:) name:UIKeyboardDidShowNotification object:nil];
        [c addObserver:self selector:@selector(handleKeyboardWillShow:) name:UIKeyboardWillShowNotification object:nil];
    }
    else
    {
        [c removeObserver:self name:UIKeyboardWillHideNotification object:nil];
        [c removeObserver:self name:UIKeyboardDidHideNotification object:nil];
        [c removeObserver:self name:UIKeyboardDidShowNotification object:nil];
        [c removeObserver:self name:UIKeyboardWillShowNotification object:nil];
    }
}

- (CGFloat)keyboardHeight
{
    return MIN (keyboardSize.height, keyboardSize.width);
}

- (void)handleKeyboardWillShow:(NSNotification *)notification
{
    self.lastNotification = notification;
    
    
    self.lastNotification = nil;
}

- (void)handleKeyboardDidShow:(NSNotification *)notification
{
    self.lastNotification = notification;

    [self adjustTextView];
    
    self.lastNotification = nil;
}

- (void)handleKeyboardWillHide:(NSNotification *)notification
{
    self.lastNotification = notification;
    
    [self resetTextView];
    
    self.lastNotification = nil;
}

- (void)handleKeyboardDidHide:(NSNotification *)notification
{
    self.lastNotification = notification;
    
    
    self.lastNotification = nil;
}

- (void)adjustTextView
{
    if (self.lastNotification != nil)
    {
        CGRect keyboardRect;
        [[[self.lastNotification userInfo] objectForKey:UIKeyboardFrameBeginUserInfoKey] getValue:&keyboardRect];
        CGRect keyFrame = [[UIApplication sharedApplication].keyWindow.rootViewController.view convertRect:keyboardRect fromView:nil];
        CGSize kbSize = keyFrame.size;
        
        UIInterfaceOrientation orientation = [UIApplication sharedApplication].statusBarOrientation;
        if (orientation == UIInterfaceOrientationLandscapeLeft || orientation == UIInterfaceOrientationLandscapeRight )
        {
            CGSize origKeySize = kbSize;
            kbSize.height = origKeySize.width;
            kbSize.width = origKeySize.height;
        }
        
        keyboardSize = kbSize;
        if (self.textView != nil)
        {
            UIEdgeInsets contentInsets = UIEdgeInsetsMake (0.0f, 0.0f, kbSize.height, 0.0f);
            self.textView.contentInset = contentInsets;
            self.textView.scrollIndicatorInsets = contentInsets;
        }
    }
}

- (void)resetTextView
{
    keyboardSize.width = 0;
    keyboardSize.height = 0;
    if (self.textView != nil)
    {
        UIEdgeInsets contentInsets = UIEdgeInsetsZero;
        self.textView.contentInset = contentInsets;
        self.textView.scrollIndicatorInsets = contentInsets;
    }
}

@end
