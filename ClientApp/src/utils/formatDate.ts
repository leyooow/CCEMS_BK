import moment from 'moment';
import { DATE_FORMAT } from '../utils/constants'


export const FormattedDate = (date: string) => {
    return moment(date).format(DATE_FORMAT);
};