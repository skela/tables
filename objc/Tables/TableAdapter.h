//
//  TableAdapter.h
//  tables
//
//  Created by Aleksander Slater on 07/09/2014.
//  Copyright (c) 2014 Davincium. All rights reserved.
//

#import <Foundation/Foundation.h>

#import "TableUtils.h"

@interface TableAdapter : NSObject <ITableAdapter,UITableViewDataSource,UITableViewDelegate>
@property(assign) id<ITableAdapterRowSelector>rowSelector;
@property(nonatomic,strong) id<ITableAdapterRowConfigurator>rowConfigurator;
@property(nonatomic,strong) id<ITableAdapterRowChanged>rowChanged;

@property(nonatomic,readwrite) BOOL shouldAdjustTextContentInset;
@property(nonatomic,strong) UIColor *detailTextColor;

@end

