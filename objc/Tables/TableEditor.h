//
//  TableEditor.h
//  tables
//
//  Created by Aleksander Slater on 07/09/2014.
//  Copyright (c) 2014 Davincium. All rights reserved.
//

#import <UIKit/UIKit.h>

@interface TableEditor : UIViewController

@end

@interface TableAdapterInlineTextInputAccessoryView : UIView
@property(nonatomic,strong) UIButton *nextButton;
@property(nonatomic,strong) UIButton *previousButton;
@property(nonatomic,strong) UIButton *dismissButton;
@property(nonatomic,strong) NSIndexPath *indexPath;
@end
