//
//  Tables.h
//  tables
//
//  Created by Aleksander Slater on 07/09/2014.
//  Copyright (c) 2014 Davincium. All rights reserved.
//

#import <Foundation/Foundation.h>

#import "TableAdapter.h"
#import "TableUtils.h"

typedef enum
{
    TestDataItemOptionNothing,
    TestDataItemOptionCats,
    TestDataItemOptionPizza
} TestDataItemOption;

@interface TestData : NSObject <ITableAdapterRowConfigurator>
@property (nonatomic,strong, readonly) NSString*version;
@property (nonatomic,strong) NSString*build;
@property (nonatomic,strong) NSString*name;
@property (nonatomic,strong) NSString*telephone;
@property (nonatomic,strong) NSString*password;
@property (nonatomic,strong) NSString*stuff;
@property (nonatomic,strong) NSString*stuff2;
@property (nonatomic,readwrite) BOOL chosen;
@property (nonatomic,readwrite) BOOL cool;
@property (nonatomic,readwrite) TestDataItemOption singleChoice;
@property (nonatomic,strong) NSDate*time1;
@property (nonatomic,strong) NSDate*time2;
@property (nonatomic,strong) NSDate*time3;

+ (TestData*)createTestData;

@end