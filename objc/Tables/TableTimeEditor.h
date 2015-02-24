//
//  TableTimeEditor.h
//  tables
//
//  Created by Aleksander Slater on 08/09/2014.
//  Copyright (c) 2014 Davincium. All rights reserved.
//

#import <UIKit/UIKit.h>
#import "TableEditor.h"

@interface TableTimeEditor : TableEditor
- (id)initWithTitle:(NSString*)aTitle value:(NSDate*)aValue mode:(UIDatePickerMode)aMode changed:(void (^)(NSDate *date))block;
@end
