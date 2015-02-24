//
//  TableSingleChoiceEditor.h
//  tables
//
//  Created by Aleksander Slater on 08/09/2014.
//  Copyright (c) 2014 Davincium. All rights reserved.
//

#import <UIKit/UIKit.h>
#import "TableEditor.h"

@interface TableSingleChoiceEditor : TableEditor <UIPickerViewDataSource,UIPickerViewDelegate>
- (id)initWithRowType:(TableRowType)type title:(NSString*)aTitle chosenOption:(NSObject*)chosenOption config:(TableAdapterRowConfig*)config changed:(void (^)(NSObject *object))block;
@end
