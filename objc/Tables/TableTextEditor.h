//
//  TableTextEditor.h
//  tables
//
//  Created by Aleksander Slater on 08/09/2014.
//  Copyright (c) 2014 Davincium. All rights reserved.
//

#import <UIKit/UIKit.h>
#import "TableEditor.h"

@interface TableTextEditor : TableEditor <UITextFieldDelegate>
{
    UITextAutocapitalizationType capitalizationType;
    UITextAutocorrectionType correctionType;
    UIKeyboardType keyboardType;
    
    BOOL secureTextEntry;
    
    CGSize keyboardSize;
}

@property(nonatomic,readwrite) BOOL shouldAdjustTextContentInset;

- (id)initWithRowType:(TableRowType)type title:(NSString*)aTitle value:(NSString*)aValue changed:(void (^)(NSString *text))block;
- (void)configure:(TableAdapterRowConfig*)config;

@end
