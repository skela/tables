//
//  TableSource.m
//  tables
//
//  Created by Aleksander Slater on 07/09/2014.
//  Copyright (c) 2014 Davincium. All rights reserved.
//

#import "TableSource.h"

#import <objc/runtime.h>

@interface TableSource()
@property(nonatomic,strong) NSObject*data;
@property(nonatomic,readwrite) TableRowType defaultStringRowType;

@property(nonatomic,strong) NSMutableDictionary*formatters;
@end

@implementation TableSource

- (id)initWithData:(NSObject*)data defaultStringRowType:(TableRowType)rowType
{
    self = [super init];
    if (self)
    {
        self.formatters = [[NSMutableDictionary alloc] init];
        
        self.data = data;
        self.defaultStringRowType= rowType;
    }
    return self;
}


- (id)initWithData:(NSObject*)data
{
    self = [self initWithData:data defaultStringRowType:TableRowTypeText];
    if (self)
    {
        
    }
    return self;
}

- (NSDateFormatter*)formatterForFormat:(NSString*)format
{
    NSDateFormatter *f = [self.formatters objectForKey:format];
    if (f==nil)
    {
        f = [[NSDateFormatter alloc] init];
        [f setDateFormat:format];
        [self.formatters setObject:f forKey:format];
    }
    return f;
}

- (NSInteger)numberOfSections
{
    return 1;
}

- (NSInteger)rowsInSection:(NSInteger)section
{
    if (self.data != nil)
    {
        unsigned int numberOfProperties = 0;
        objc_property_t * plist = class_copyPropertyList(object_getClass(self.data), &numberOfProperties);
        free(plist);
        return numberOfProperties;
    }
    return 0;
}

- (NSObject*)getValueAtRow:(NSInteger)row andSection:(NSInteger)section
{
    return nil;
}

- (void)setValue:(NSObject*)obj atRow:(NSInteger)row andSection:(NSInteger)section
{
    
}

- (NSString*)displayName:(id<ITableAdapterRowConfigurator>)configurator row:(NSInteger)row section:(NSInteger)section
{
    return nil;
}

- (NSString*)displayDate:(id<ITableAdapterRowConfigurator>)configurator row:(NSInteger)row section:(NSInteger)section date:(NSDate*)date rowType:(TableRowType)rowType
{
    NSString *propertyName = [self rowName:row andSection:section];
    TableAdapterRowConfig *s = [self rowSetting:configurator propertyName:propertyName];
    NSString *format = nil;
    switch (rowType)
    {
        case TableRowTypeDate:
            format = @"d MMM yyyy";
            break;
        case TableRowTypeTime:
            format = @"HH:mm";
            break;
        case TableRowTypeDateTime:
            format = @"HH:mm d MMM yyyy";
            break;
        default: break;
    }
    if (s != nil)
    {
        switch (rowType)
        {
            case TableRowTypeDate:
                if (s.dateFormat!=nil)
                    format = s.dateFormat;
                break;
            case TableRowTypeTime:
                if (s.timeFormat!=nil)
                    format = s.timeFormat;
                break;
            case TableRowTypeDateTime:
                if (s.dateTimeFormat!=nil)
                    format = s.dateTimeFormat;
                break;
            default: break;
        }
    }
    return format == nil ? [date description] : [[self formatterForFormat:format] stringFromDate:date];
}

- (NSString*)rowName:(NSInteger)row andSection:(NSInteger)section
{
    NSString *propertyName = nil; // TODO: Get Property Name
    return propertyName;
}

- (TableAdapterRowConfig*)rowSetting:(id<ITableAdapterRowConfigurator>)configurator propertyName:(NSString*)propertyName
{
    TableAdapterRowConfig *settings = nil;
    if ([self.data respondsToSelector:@selector(configForRow:)])
    {
        id<ITableAdapterRowConfigurator>temp = (id<ITableAdapterRowConfigurator>)self.data;
        settings = [temp configForRow:propertyName];
    }
    else if (configurator != nil)
        settings = [configurator configForRow:propertyName];
    return settings;
}

- (TableRowType)rowType:(id<ITableAdapterRowConfigurator>)configurator row:(NSInteger)row section:(NSInteger)section
{
    NSObject *prop = nil; // TODO: Get Property
    NSString *propertyName = [self rowName:row andSection:section];
    TableAdapterRowConfig*s = [self rowSetting:configurator propertyName:propertyName];
    if (s != nil && s.rowType != TableRowTypeUnknown)
        return s.rowType;
    TableRowType rowType = self.defaultStringRowType;
    if ([prop isKindOfClass:[NSString class]])
        rowType = self.defaultStringRowType;
    else if ([prop isKindOfClass:[NSNumber class]])
        rowType = TableRowTypeCheckbox;
    else if ([prop isKindOfClass:[NSDate class]])
        rowType = TableRowTypeDateTime;
    return rowType;
}

- (BOOL)rowEditable:(id<ITableAdapterRowConfigurator>)configurator row:(NSInteger)row section:(NSInteger)section
{
    NSObject *prop = nil; // TODO: Get Property
    NSString *propertyName = [self rowName:row andSection:section];
    //var editable = prop.CanWrite && prop.SetMethod.IsPublic;
    BOOL editable = YES;
    TableAdapterRowConfig *settings = [self rowSetting:configurator propertyName:propertyName];
    if (settings!=nil)
        editable = settings.editable;
    return editable;
}

@end
