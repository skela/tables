//
//  TableEditor.h
//  tables
//
//  Created by Aleksander Slater on 07/09/2014.
//  Copyright (c) 2014 Davincium. All rights reserved.
//

#import <UIKit/UIKit.h>
#import "TableUtils.h"

@class TableAdapterRowConfig;
@interface TableEditor : UIViewController
+ (void)configureTextControl:(TableAdapterRowConfig*)config control:(id<UITextInputTraits>)control;
+ (UIKeyboardType)convertKeyboardType:(KeyboardType)kbType;
+ (UITextAutocapitalizationType)convertCapitatilizationType:(CapitalizationType)capType;
+ (UITextAutocorrectionType)convertCorrectionType:(CorrectionType)correctionType;

- (void)closeViewController;;

@end

@class TableAdapterRowConfig;
@interface TableAdapterInlineTextInputAccessoryView : UIView
@property(nonatomic,strong) UIButton *nextButton;
@property(nonatomic,strong) UIButton *previousButton;
@property(nonatomic,strong) UIButton *dismissButton;
@property(nonatomic,strong) NSIndexPath *indexPath;
- (id)initWithConfig:(TableAdapterRowConfig*)config andWidth:(CGFloat)width;
@end
