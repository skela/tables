//
//  TableEditor.m
//  tables
//
//  Created by Aleksander Slater on 07/09/2014.
//  Copyright (c) 2014 Davincium. All rights reserved.
//

#import "TableEditor.h"

@interface TableEditor ()

@end

@implementation TableEditor

- (id)init
{
    self = [super init];
    if (self)
    {
        self.hidesBottomBarWhenPushed = YES;
    }
    return self;
}

- (BOOL)extendedLayoutIncludesOpaqueBars
{
    return YES;
}

- (BOOL)isModal
{
    if (self.navigationController == nil || self.navigationController.viewControllers.count == 1)
        return true;
    return false;
}

- (void)closeViewController
{
    if (self.isModal)
    {
        [self dismissViewControllerAnimated:YES completion:nil];
    }
    else
    {
        [self.navigationController popViewControllerAnimated:YES];
    }
}

+ (UIKeyboardType)convertKeyboardType:(KeyboardType)kbType
{
    switch (kbType)
    {
        case KeyboardTypeDefault:
            return UIKeyboardTypeDefault;
        case KeyboardTypeASCIICapable:
            return UIKeyboardTypeASCIICapable;
        case KeyboardTypeNumbersAndPunctuation:
            return UIKeyboardTypeNumbersAndPunctuation;
        case KeyboardTypeUrl:
            return UIKeyboardTypeURL;
        case KeyboardTypeNumberPad:
            return UIKeyboardTypeNumberPad;
        case KeyboardTypePhonePad:
            return UIKeyboardTypePhonePad;
        case KeyboardTypeNamePhonePad:
            return UIKeyboardTypeNamePhonePad;
        case KeyboardTypeEmailAddress:
            return UIKeyboardTypeEmailAddress;
        case KeyboardTypeDecimalPad:
            return UIKeyboardTypeDecimalPad;
        case KeyboardTypeIgnore:
            return UIKeyboardTypeDefault;
    }
}

+ (UITextAutocapitalizationType)convertCapitatilizationType:(CapitalizationType)capType
{
    switch (capType)
    {
        case CapitalizationTypeNone:
            return UITextAutocapitalizationTypeNone;
        case CapitalizationTypeWords:
            return UITextAutocapitalizationTypeWords;
        case CapitalizationTypeSentences:
            return UITextAutocapitalizationTypeSentences;
        case CapitalizationTypeAllCharacters:
            return UITextAutocapitalizationTypeAllCharacters;
        case CapitalizationTypeIgnore:
            return UITextAutocapitalizationTypeNone;
    }
}

+ (UITextAutocorrectionType)convertCorrectionType:(CorrectionType)correctionType
{
    switch (correctionType)
    {
        case CorrectionTypeDefault:
            return UITextAutocorrectionTypeDefault;
        case CorrectionTypeNo:
            return UITextAutocorrectionTypeNo;
        case CorrectionTypeYes:
            return UITextAutocorrectionTypeYes;
        case CorrectionTypeIgnore:
            return UITextAutocorrectionTypeDefault;
    }
}

+ (void)configureTextControl:(TableAdapterRowConfig*)config control:(id<UITextInputTraits>)control
{
    if (config!=nil && control!=nil)
    {
        if (config.keyboardType != KeyboardTypeIgnore)
            control.keyboardType = [TableEditor convertKeyboardType:config.keyboardType];
        if (config.capitalizationType != CapitalizationTypeIgnore)
            control.autocapitalizationType = [TableEditor convertCapitatilizationType:config.capitalizationType];
        if (config.correctionType != CorrectionTypeIgnore)
            control.autocorrectionType = [TableEditor convertCorrectionType:config.correctionType];
    }
}

@end

@implementation TableAdapterInlineTextInputAccessoryView

- (id)initWithConfig:(TableAdapterRowConfig*)config andWidth:(CGFloat)width
{
    self = [super initWithFrame:CGRectMake(0, 0, width, 40)];
    if (self)
    {
        self.autoresizingMask = UIViewAutoresizingFlexibleWidth;
        self.backgroundColor = [UIColor colorWithRed:209 green:213 blue:218 alpha:1];
        UIColor* textColor = [UIColor blackColor];
        self.nextButton = [[UIButton alloc] initWithFrame:CGRectMake(0, 0, 40, 40)];
        self.previousButton = [[UIButton alloc] initWithFrame:CGRectMake(0, 0, 40, 40)];
        self.dismissButton = [[UIButton alloc] initWithFrame:CGRectMake(0, 0, 40, 40)];
        [self.previousButton setTitle:@"\u25C4" forState:UIControlStateNormal];
        [self.nextButton setTitle:@"\u25BA" forState:UIControlStateNormal];
        [self.dismissButton setTitle:@"\u2637" forState:UIControlStateNormal];
        [self.nextButton setTitleColor:textColor forState:UIControlStateNormal];
        [self.previousButton setTitleColor:textColor forState:UIControlStateNormal];
        [self.dismissButton setTitleColor:textColor forState:UIControlStateNormal];
        [self addSubview:self.nextButton];
        [self addSubview:self.previousButton];
        [self addSubview:self.dismissButton];
    }
    return self;
}

- (void)layoutSubviews
{
    [super layoutSubviews];
    self.previousButton.frame = CGRectMake(10, 0, 40, 40);
    self.nextButton.frame = CGRectMake(self.previousButton.frame.size.width+20, 0, 40, 40);
    self.dismissButton.frame = CGRectMake(self.frame.size.width-10-40, 0, 40, 40);
}

@end
