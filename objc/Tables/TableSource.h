//
//  TableSource.h
//  tables
//
//  Created by Aleksander Slater on 07/09/2014.
//  Copyright (c) 2014 Davincium. All rights reserved.
//

#import <Foundation/Foundation.h>

#import "TableUtils.h"

@protocol ITableSource
- (NSObject*)getValueAtRow:(NSInteger)row andSection:(NSInteger)section;
- (NSInteger)rowsInSection:(NSInteger)section;
- (NSInteger)numberOfSections;
@end

@interface TableSource : NSObject <ITableSource>
- (id)initWithData:(NSObject*)data;
- (id)initWithData:(NSObject*)data defaultStringRowType:(TableRowType)rowType;

- (NSObject*)getValueAtRow:(NSInteger)row andSection:(NSInteger)section;

- (NSString*)displayName:(id<ITableAdapterRowConfigurator>)configurator row:(NSInteger)row section:(NSInteger)section;

- (NSString*)rowName:(NSInteger)row andSection:(NSInteger)section;
- (TableAdapterRowConfig*)rowSetting:(id<ITableAdapterRowConfigurator>)configurator propertyName:(NSString*)propertyName;
- (TableRowType)rowType:(id<ITableAdapterRowConfigurator>)configurator row:(NSInteger)row section:(NSInteger)section;
- (BOOL)rowEditable:(id<ITableAdapterRowConfigurator>)configurator row:(NSInteger)row section:(NSInteger)section;
@end
